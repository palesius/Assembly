using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Assembly.Helpers;
using Assembly.Metro.Controls.PageTemplates.Games.Components;
using Assembly.Metro.Controls.PageTemplates.Games.Components.Editors;
using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using Assembly.Metro.Dialogs;
using Assembly.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using Blamite.Blam;
using Blamite.Blam.Localization;
using Blamite.Blam.Resources;
using Blamite.Blam.Scripting;
using Blamite.Serialization;
using Blamite.Injection;
using Blamite.IO;
using Blamite.Plugins;
using Blamite.RTE;
//using Blamite.RTE.H2Vista;
using Blamite.Util;
using CloseableTabItemDemo;
using Microsoft.Win32;
using Newtonsoft.Json;
using XBDMCommunicator;
using Blamite.Blam.ThirdGen;
//using Blamite.RTE.MCC;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Resources;
using System.Windows.Threading;
using Assembly.Helpers.Native;
using Assembly.Helpers.Net;
using Assembly.Metro.Controls.PageTemplates;
using Assembly.Helpers.Plugins;
using Assembly.Metro.Controls.PageTemplates.Games;
using Assembly.Metro.Controls.PageTemplates.Tools;
using Assembly.Metro.Controls.PageTemplates.Tools.Halo4;
using XboxChaos.Models;
using Xceed.Wpf.AvalonDock.Controls;
using Assembly.Helpers.Net.Sockets;
using Blamite.Blam.ThirdGen.Structures;

namespace Assembly.Windows
{
	public static class ExtensionMethods
	{
		private static readonly Action EmptyDelegate = delegate { };
		public static void Refresh(this UIElement uiElement)
		{
			uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
		}
	}

	public partial class WSTagGroup
	{
		public String name;
		public Dictionary<String, WSTagEntry> entries = new Dictionary<String, WSTagEntry>();

		public WSTagGroup(String _name)
		{
			name = _name;
		}
	}

	public partial class WSTagEntry
	{
		public String name;
		public Dictionary<uint, WSTagField> fields=new Dictionary<uint, WSTagField>();

		public WSTagEntry(String _name)
		{
			name = _name;
		}

		public void addField(String def,bool mapLevel)
		{
			WSTagField f = new WSTagField(def);
			if (mapLevel && fields.ContainsKey(f.line)) fields.Remove(f.line);
			fields.Add(f.line, f);
		}

		public void cloneFields(WSTagEntry src)
		{
			foreach (WSTagField tf in src.fields.Values)
			{
				fields.Add(tf.line, tf);
			}
		}
	}

	public class WSTagField
	{
		public int hits=0;
		public tfType fldType=tfType.unknown;
		public uint line;
		public string name;
		public float valFloat;
		public int valInt;
		public bool valFlagType;
		public enum tfType : int
		{
			unknown = -1,
			float32 = 0,
			flags32 = 1,
			int16 = 2,
			enum8 = 3
		}

		public static WSTagField.tfType nameToEnum(String name)
		{
			switch (name)
			{
				case "float32": return tfType.float32;
				case "flags32": return tfType.flags32;
				case "int16": return tfType.int16;
				case "enum8": return tfType.enum8;
			}
			return tfType.unknown;
		}

		public static String enumToType(WSTagField.tfType fldType)
		{
			switch (fldType)
			{
				case tfType.float32: return "Float32Data";
				case tfType.flags32: return "FlagData";
				case tfType.int16: return "Int16Data";
				case tfType.enum8: return "EnumData";
			}
			return null;
		}

		public WSTagField(String src)
		{
			String[] toks=src.Split(':');
			fldType = WSTagField.nameToEnum(toks[0]);
			line = uint.Parse(toks[1]);
			name = toks[2];
			switch (fldType) 
			{
				case tfType.float32:
					valFloat = float.Parse(toks[3]);
					break;
				case tfType.int16:
					valInt = int.Parse(toks[3]);
					break;
				case tfType.flags32:
					valInt = int.Parse(toks[3].Substring(1));
					switch (toks[3].Substring(0,1))
					{
						case "+":valFlagType = true;break;
						case "-":valFlagType = false;break;
						default:valFlagType = true;break;
					}
					break;
				case tfType.enum8:
					valInt = int.Parse(toks[3]);
					break;
				default:
					break;
			}
		}

		public WSTagField(WSTagField tf)
		{
			fldType = tf.fldType;
			line = tf.line;
			name = tf.name;
			valFloat = tf.valFloat;
			valInt = tf.valInt;
			valFlagType = tf.valFlagType;
		}

		public String getFldTypeName()
		{
			return WSTagField.enumToType(fldType);
		}
	}

	public partial class Home
	{
		private Dictionary<String, WSTagGroup> loadWSSettings(HaloMap map)
		{
			Dictionary<String, WSTagGroup> tgDict = new Dictionary<string, WSTagGroup>();
			String genPath = string.Format("g:\\th\\reach\\wsset\\{0}.txt", map.GetBuildInfo().GameModule);
			String mapPath = string.Format("g:\\th\\reach\\wsset\\{0}_{1}.txt", map.GetBuildInfo().GameModule, map.GetCacheFile().InternalName);
			if (System.IO.File.Exists(genPath))
			{
				String[] lines = System.IO.File.ReadAllLines(genPath);
				for (int i = 0; i < lines.Count(); i++)
				{
					String[] toks = lines[i].Split('|');g:
					WSTagGroup tg = null;
					if (!tgDict.TryGetValue(toks[0], out tg))
					{
						tg = new WSTagGroup(toks[0]);
						tgDict.Add(toks[0], tg);
					}
					WSTagEntry te = null;
					if (!tg.entries.TryGetValue(toks[1], out te))
					{
						te = new WSTagEntry(toks[1]);
						tg.entries.Add(toks[1], te);
					}
					if (toks[2].Substring(0, 1) == "*")
					{
						WSTagEntry src = null;
						if (tg.entries.TryGetValue(toks[2], out src)) { te.cloneFields(tg.entries[(toks[2])]); }
					}
					else
					{
						te.addField(toks[2],false);
					}
				}
			}
			if (System.IO.File.Exists(mapPath))
			{
				String[] lines = System.IO.File.ReadAllLines(mapPath);
				for (int i = 0; i < lines.Count(); i++)
				{
					String[] toks = lines[i].Split('|');
					WSTagGroup tg = null;
					if (!tgDict.TryGetValue(toks[0], out tg))
					{
						tg = new WSTagGroup(toks[0]);
						tgDict.Add(toks[0], tg);
					}
					WSTagEntry te = null;
					if (!tg.entries.TryGetValue(toks[1], out te))
					{
						te = new WSTagEntry(toks[1]);
						tg.entries.Add(toks[1], te);
					}
					if (toks[2].Substring(0, 1) == "*")
					{
						WSTagEntry src = null;
						if (tg.entries.TryGetValue(toks[2], out src)) { te.cloneFields(tg.entries[(toks[2])]); }
					}
					else
					{
						te.addField(toks[2],true);
					}
				}
			}
			return tgDict;
		}

		private void menuWalkingSim_Click(object sender, RoutedEventArgs e)
		{
			frmStatus frmStat = new frmStatus();
			frmStat.Show();
			int mapCnt = 0;
			int mapIdx = 0;
			List<String> lstIssues = new List<String>();
			foreach (LayoutDocument ld in documentManager.Children)
			{
				if (ld.Content.GetType().Name == "HaloMap") { mapCnt++; }
			}
			foreach (LayoutDocument ld in documentManager.Children)
			{
				if (ld.Content.GetType().Name == "HaloMap")
				{
					HaloMap map = (HaloMap)ld.Content;
					mapIdx++;
					String mapMsg;
					String mapPath = map.GetCacheLocation();
					String bakPath = mapPath.Substring(0, mapPath.Length - 4) + ".bak";
					if (!System.IO.File.Exists(bakPath)) {
						mapMsg = string.Format("({0}/{1}) {2} - Creating backup", mapIdx, mapCnt, map.GetCacheFile().InternalName);
						frmStat.UpdateMapStatus(mapIdx, mapCnt, mapMsg);
						System.IO.File.Copy(mapPath, bakPath); }
					Dictionary<String, WSTagGroup> wsDict = loadWSSettings(map);
					mapMsg= string.Format("({0}/{1}) {2} - Scanning Groups", mapIdx, mapCnt, map.GetCacheFile().InternalName);
					frmStat.UpdateMapStatus(mapIdx, mapCnt, mapMsg);
					foreach (TagGroup tg in map.tvTagList.Items)
					{
						WSTagGroup wstg = null;
						if (wsDict.TryGetValue(tg.TagGroupMagic,out wstg))
						{
							mapMsg = string.Format("({0}/{1}) {2} - Processing {3}", mapIdx, mapCnt, map.GetCacheFile().InternalName,tg.TagGroupMagic);
							frmStat.UpdateMapStatus(mapIdx, mapCnt, mapMsg);
							int tagCnt = tg.Children.Count;
							int tagIdx = 0;
							foreach (TagEntry te in tg.Children)
							{
								tagIdx++;
								WSTagEntry wste = null;
								if (!wstg.entries.TryGetValue(te.TagFileName,out wste))
								{
									int prefPos = 0;
									while (prefPos<te.TagFileName.Length && prefPos>=0)
									{
										WSTagEntry tryWste = null;
										wstg.entries.TryGetValue(te.TagFileName.Substring(0,prefPos)+"*", out tryWste);
										if (tryWste != null)
										{
											Debug.Print(te.TagFileName + "<=>" + te.TagFileName.Substring(0, prefPos) + "*");
											wste = tryWste;
										}
										prefPos = te.TagFileName.IndexOf('\\', prefPos);
										if (prefPos > 0) prefPos++;
									}
								}
								if (wste!=null) {
									String tagMsg = string.Format("({0}/{1}) {2}", tagIdx, tagCnt, te.TagFileName);
									frmStat.UpdateTagStatus(tagIdx, tagCnt, tagMsg);
									processTagEntry(te, wste, map,lstIssues); 
								}
							}

						}
					}
					foreach (WSTagGroup wstg in wsDict.Values)
					{
						foreach (WSTagEntry wste in wstg.entries.Values)
						{
							foreach (WSTagField wstf in wste.fields.Values)
							{
								if (wstf.hits==0)
								{
									lstIssues.Add(string.Format("No hits for [{0}]{1}: ({2}) \"{3}\"", wstg.name, wste.name, wstf.line, wstf.name));
								}
							}
						}

					}
				}
			}
			frmStat.Close();

			if (lstIssues.Count>0)
			{
				System.Text.StringBuilder sbIssues = new System.Text.StringBuilder();
				foreach (String curIssue in lstIssues) { sbIssues.AppendLine(curIssue); }
				Clipboard.SetText(sbIssues.ToString());
				MessageBox.Show("Some weapons/equipment were not included in the settings files. Copied to clipboard.");
			}
		}

		private void processTagEntry(TagEntry te, WSTagEntry wste,HaloMap map,List<string> lstIssues)
		{
			map.CreateTag(te);
			CloseableTabItem cti = (CloseableTabItem)map.contentTabs.SelectedItem;
			MetaContainer mc = (MetaContainer)cti.Content;
			MetaEditor me = (MetaEditor)mc.tabMetaEditor.Content;
			ThirdGenPluginVisitor tgp = me.GetPluginVisitor();
			bool dirty = false;
			foreach (MetaField mfouter in tgp.Values)
			{
				MetaField mf = mfouter;
				while (mf.GetType().Name == "WrappedTagBlockEntry")
				{
					mf = ((WrappedTagBlockEntry)mf).WrappedField;
				}
				WSTagField wstf = null;
				if (wste.fields.TryGetValue(mf.PluginLine,out wstf))
				{
					if (wstf.getFldTypeName()==mf.GetType().Name)
					{
						switch (wstf.fldType)
						{
							case WSTagField.tfType.float32:
								Float32Data dFloat32 = (Float32Data)mf;
								if (dFloat32.Name == wstf.name)
								{
									wstf.hits += 1;
									if (dFloat32.Value != wstf.valFloat)
									{
										dirty = true;
										dFloat32.Value = wstf.valFloat;
									}
								}
								break;
							case WSTagField.tfType.flags32:
								FlagData dFlag = (FlagData)mf;
								if (dFlag.Name == wstf.name)
								{
									wstf.hits += 1;
									if (dFlag.Bits.ElementAt(wstf.valInt).IsSet != wstf.valFlagType)
									{
										dirty = true;
										dFlag.Bits.ElementAt(wstf.valInt).IsSet = wstf.valFlagType;
									}
								}
								break;
							case WSTagField.tfType.int16:
								Int16Data dInt16 = (Int16Data)mf;
								if (dInt16.Name == wstf.name)
								{
									wstf.hits += 1;
									if (dInt16.Value != wstf.valInt)
									{
										dirty = true;
										dInt16.Value = (short)wstf.valInt;
									}
								}
								break;
							case WSTagField.tfType.enum8:
								EnumData dEnum = (EnumData)mf;
								if (dEnum.Name == wstf.name)
								{
									wstf.hits += 1;
									if (dEnum.Value != wstf.valInt)
									{
										dirty = true;
										dEnum.Value = wstf.valInt;
									}
								}
								break;
						}

					}
				}
			}
			if (dirty) {
				Debug.Print("Updated [" + te.GroupName + "]:" + te.TagFileName + ".");
				me.PublicSave(); 
			}
			RaiseEvent(new RoutedEventArgs(CloseableTabItem.CloseTabEvent, cti));
			map.contentTabs.Items.Remove(cti);
		}

		private class PathProcess
		{
			public enum ppStatus : int
			{
				ppsIdle =1,
				ppsStarted=2,
				ppsComplete=3
			}
			public Process p;
			public String mapPath;
			public String bakPath;
			public String patPath;

			public ppStatus status;
			public PathProcess(String _mapPath)
			{
				mapPath = (String)_mapPath.Clone();
				bakPath = mapPath.Substring(0, mapPath.Length - 4) + ".bak";
				patPath = mapPath.Substring(0, mapPath.Length - 4) + ".pat";
				status = ppStatus.ppsIdle;
				p = new System.Diagnostics.Process();
				p.StartInfo.FileName = "g:\\th\\reach\\VPatch32\\GenPat.exe";
				p.StartInfo.Arguments = "\"" + bakPath + "\" \"" + mapPath + "\" \"" + patPath + "\" /b=16";
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				p.StartInfo.CreateNoWindow = true;
			}

			public void Start()
			{
				p.Start();
				status = ppStatus.ppsStarted;
			}

			public void Complete()
			{
				p.Dispose();
				System.IO.File.Delete(mapPath);
				System.IO.File.Move(bakPath, mapPath);
				status = ppStatus.ppsComplete;
			}
		}
		private void menuWSComplete_Click(object sender, RoutedEventArgs e)
		{
			int mapCnt = 0;
			int mapIdx = 0;
			foreach (LayoutDocument ld in documentManager.Children)
			{
				if (ld.Content.GetType().Name == "HaloMap") { mapCnt++; }
			}
			frmStatus frmStat = new frmStatus();
			frmStat.Show();
			List<PathProcess> mapProcesses = new List<PathProcess>();
			foreach (LayoutDocument ld in documentManager.Children)
			{
				if (ld.Content.GetType().Name == "HaloMap")
				{
					mapIdx++;
					HaloMap map = (HaloMap)ld.Content;
					PathProcess p = new PathProcess(map.GetCacheLocation());
					mapProcesses.Add(p);
				}
			}
			int inactiveCount = mapProcesses.Count;
			int activeCount = 0;
			int maxActive = 4;
			String statString;

			while (inactiveCount > 0 || activeCount>0)
			{
				statString = "Generating Patches: ";
				foreach (PathProcess p in mapProcesses)
				{
					statString += ((int)p.status).ToString();
					switch (p.status)
					{
						case PathProcess.ppStatus.ppsIdle:
							if (activeCount<maxActive)
							{
								p.Start();
								activeCount += 1;
								inactiveCount -= 1;
							}
							break;
						case PathProcess.ppStatus.ppsStarted:
							if (p.p.HasExited)
							{
								p.Complete();
								activeCount -= 1;
							}
							break;
						case PathProcess.ppStatus.ppsComplete:
							break;
					}
				}
				frmStat.UpdateMapStatus(0, 1, statString);
				System.Threading.Thread.Sleep(500);
			}

			frmStat.Close();
		}
	}
}

namespace Assembly.Metro.Controls.PageTemplates.Games
{
	public partial class HaloMap : INotifyPropertyChanged
	{
		public string GetCacheLocation()
		{
			return _cacheLocation;
		}
		public ICacheFile GetCacheFile()
		{
			return _cacheFile;
		}
		public EngineDescription GetBuildInfo()
		{
			return _buildInfo;
		}

		public void saveCacheFile()
		{
			using (IStream stream = _mapManager.OpenReadWrite())
				_cacheFile.SaveChanges(stream);
		}

		public void WSForceLoad(TagEntry tag)
		{
			var container = extractTags(new List<TagEntry>() { tag }, ExtractMode.Forceload, true, false);

			if (container == null)
				return;

			// Now take the info we just extracted and use it to forceload
			bool dirty = false;
			using (IStream stream = _mapManager.OpenReadWrite())
			{
				var _zonesets = _cacheFile.Resources.LoadZoneSets(stream);

				foreach (ExtractedTag et in container.Tags)
				{
					if (!_zonesets.GlobalZoneSet.IsTagActive(et.OriginalIndex))
					{
						_zonesets.GlobalZoneSet.ActivateTag(et.OriginalIndex, true);
						dirty = true;
					}
				}

				foreach (ExtractedResourceInfo eri in container.Resources)
				{
					if (!_zonesets.GlobalZoneSet.IsResourceActive(eri.OriginalIndex))
					{
						_zonesets.GlobalZoneSet.ActivateResource(eri.OriginalIndex, true);
						dirty = true;
					}
				}

				if (dirty)
				{
					_zonesets.SaveChanges(stream);
					_cacheFile.SaveChanges(stream);
				}
			}

			if (dirty)
			{
				LoadTags();
				System.Diagnostics.Debug.Print(String.Format("Forceloaded {0}", tag.TagFileName));
			} else
			{
				System.Diagnostics.Debug.Print(String.Format("Already forceloaded {0}", tag.TagFileName));
			}
		}
	}
}

namespace Assembly.Metro.Controls.PageTemplates.Games.Components
{
	public partial class MetaEditor : UserControl
	{
		public ThirdGenPluginVisitor GetPluginVisitor()
		{
			return _pluginVisitor;
		}

		public void PublicSave()
		{
			UpdateMeta(MetaWriter.SaveType.File, false, false);
		}
	}
}