Module Program
    Sub Main(args As String())
        Dim compress As Boolean = False
        Dim src As String = vbNullString
        Dim dest As String = vbNullString
        If args.Length <> 3 Then
            printhelp()
            Exit Sub
        End If
        Select Case args(0)
            Case "-d"
                compress = False
                src = args(1)
                dest = args(2)
            Case "-c"
                compress = True
                src = args(1)
                dest = args(2)
            Case Else
                printhelp()
                Exit Sub
        End Select
        Dim state As Blamite.Compression.CompressionState
        Dim db As Blamite.Serialization.EngineDatabase = Blamite.Serialization.Settings.XMLEngineDatabaseLoader.LoadDatabase("Formats/Engines.xml")

        IO.File.Copy(src, dest, True)
        state = Blamite.Compression.CacheCompressor.HandleCompression(dest, db)
        If compress = True And state = Blamite.Compression.CompressionState.Decompressed Then
            IO.File.Delete(dest)
            Console.WriteLine("Already compressed")
        End If

        If compress = False And state = Blamite.Compression.CompressionState.Compressed Then
            IO.File.Delete(dest)
            Console.WriteLine("Already decompressed")
        End If
        Console.WriteLine("Done!")
    End Sub

    Sub printhelp()
        Console.WriteLine("Usage:")
        Console.WriteLine("Decompress: -d <input file> <output file>")
        Console.WriteLine("Compress:   -c <input file> <output file>")
    End Sub
End Module
