﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtryzeDLL.IO
{
    /// <summary>
    /// A group of file segments which share a common pointer system.
    /// </summary>
    public class FileSegmentGroup
    {
        private SortedList<int, FileSegment> _segmentsByOffset = new SortedList<int, FileSegment>();
        private IPointerConverter _pointerConverter;

        /// <summary>
        /// Occurs when a segment in the group has been resized.
        /// </summary>
        public event EventHandler<SegmentResizedEventArgs> MemberSegmentResized;

        /// <summary>
        /// Constructs a new FileSegmentGroup where pointers are equivalent to file offsets.
        /// </summary>
        public FileSegmentGroup()
        {
        }

        /// <summary>
        /// Constructs a new FileSegmentGroup.
        /// </summary>
        /// <param name="pointerConverter">The IPointerConverter to use to convert pointers to and from file offsets.</param>
        public FileSegmentGroup(IPointerConverter pointerConverter)
        {
            _pointerConverter = pointerConverter;
        }

        /// <summary>
        /// Gets the lowest possible value a valid pointer can have.
        /// </summary>
        public uint BasePointer
        {
            get
            {
                if (_pointerConverter != null)
                    return _pointerConverter.OffsetToPointer(Offset);
                return 0;
            }
        }

        /// <summary>
        /// Gets the value to subtract from a pointer to convert it to a file offset.
        /// </summary>
        public uint PointerMask
        {
            get
            {
                if (_pointerConverter != null)
                    return _pointerConverter.OffsetToPointer(0);
                return 0;
            }
        }

        /// <summary>
        /// Gets the offset of the first segment in the group.
        /// </summary>
        public int Offset
        {
            get
            {
                if (Segments.Count > 0)
                    return Segments[0].Offset;
                return 0;
            }
        }

        /// <summary>
        /// Gets the total size of the group.
        /// </summary>
        public int Size
        {
            get
            {
                if (Segments.Count > 0)
                    return (int)(Segments[Segments.Count - 1].Offset - Segments[0].Offset + Segments[Segments.Count - 1].ActualSize);
                return 0;
            }
        }

        /// <summary>
        /// Gets a list of the segments in the group sorted by their offset in the file.
        /// </summary>
        public IList<FileSegment> Segments
        {
            get { return _segmentsByOffset.Values; }
        }

        /// <summary>
        /// Adds a segment to the group.
        /// </summary>
        /// <param name="segment">The segment to add.</param>
        /// <returns>A SegmentPointer pointing to the beginning of the segment.</returns>
        public SegmentPointer AddSegment(FileSegment segment)
        {
            int offset = segment.Offset;
            if (_segmentsByOffset.ContainsKey(offset))
                throw new ArgumentException("A segment has already been added at the given offset.");

            _segmentsByOffset[segment.Offset] = segment;
            segment.Resized += SegmentResized;

            return SegmentPointer.FromOffset(segment.Offset, this);
        }

        /// <summary>
        /// Returns whether or not a given pointer falls inside the group.
        /// </summary>
        /// <param name="pointer">The pointer to test.</param>
        /// <returns>true if the pointer falls inside a segment in the group.</returns>
        public bool ContainsPointer(uint pointer)
        {
            if (Segments.Count == 0)
                return false;

            uint basePointer = BasePointer;
            return (pointer >= basePointer && pointer < basePointer + Size);
        }

        /// <summary>
        /// Returns whether or not a given offset falls inside the group.
        /// </summary>
        /// <param name="pointer">The offset to test.</param>
        /// <returns>true if the offset falls inside a segment in the group.</returns>
        public bool ContainsOffset(int offset)
        {
            return (offset >= Offset && offset < Offset + Size);
        }

        /// <summary>
        /// Finds the segment in the group which contains a given file offset.
        /// </summary>
        /// <param name="offset">The offset to search for.</param>
        /// <returns>The FileSegment containing the offset if found, or null otherwise.</returns>
        public FileSegment FindSegmentWithOffset(int offset)
        {
            // Just do a linear search for now, if this gets slow then it can be converted to binary search or something
            foreach (FileSegment segment in Segments)
            {
                if (offset >= segment.Offset && offset < segment.Offset + segment.Size)
                    return segment;
            }
            return null;
        }

        /// <summary>
        /// Finds the segment in the group which contains a given pointer.
        /// </summary>
        /// <param name="pointer">The pointer to search for.</param>
        /// <returns>The FileSegment containing the pointer if found, or null otherwise.</returns>
        public FileSegment FindSegmentWithPointer(uint pointer)
        {
            if (Segments.Count == 0)
                return null;

            // Just do a linear search for now, if this gets slow then it can be converted to binary search or something
            uint currentPointer = OffsetToPointer(Segments[0].Offset);
            foreach (FileSegment segment in Segments)
            {
                if (pointer >= currentPointer && pointer < currentPointer + segment.Size)
                    return segment;
                currentPointer += (uint)segment.Size;
            }
            return null;
        }

        /// <summary>
        /// Converts a pointer into the group to a file offset.
        /// </summary>
        /// <param name="pointer">The pointer to convert.</param>
        /// <returns>The file offset corresponding to the pointer.</returns>
        public int PointerToOffset(uint pointer)
        {
            if (_pointerConverter != null)
                return _pointerConverter.PointerToOffset(pointer);
            return (int)pointer;
        }

        /// <summary>
        /// Converts a file offset to a pointer into the group.
        /// </summary>
        /// <param name="offset">The file offset to convert.</param>
        /// <returns>The pointer corresponding to the file offset.</returns>
        public uint OffsetToPointer(int offset)
        {
            if (_pointerConverter != null)
                return _pointerConverter.OffsetToPointer(offset);
            return (uint)offset;
        }

        /// <summary>
        /// Resizes the last segment in the group, changing the total size of the group to be at least a specified size.
        /// </summary>
        /// <param name="newSize">The total amount of space that the resized group should at least occupy.</param>
        /// <param name="stream">The stream to write changes to.</param>
        public void Resize(int newSize, IStream stream)
        {
            if (Segments.Count == 0)
                return;

            FileSegment lastSegment = Segments[Segments.Count - 1];
            int newLastSegmentSize = newSize - (lastSegment.Offset - Offset);
            if (newLastSegmentSize <= 0)
                throw new ArgumentException("Cannot shrink the group enough without deleting the last segment");

            lastSegment.Resize(newLastSegmentSize, stream);
        }

        /// <summary>
        /// Raises the <see cref="MemberSegmentResized"/> event.
        /// </summary>
        /// <param name="e">The <see cref="SegmentResizedEventArgs"/> that contains the event data.</param>
        protected void OnMemberSegmentResized(SegmentResizedEventArgs e)
        {
            if (MemberSegmentResized != null)
                MemberSegmentResized(this, e);
        }

        private void SegmentResized(object sender, SegmentResizedEventArgs e)
        {
            OnMemberSegmentResized(e);
        }
    }
}
