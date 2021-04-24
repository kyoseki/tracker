using System;
using System.Collections.Generic;
using System.IO;
using kyoseki.Game.Serial;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace kyoseki.Game.Configuration
{
    public class KyosekiConfigManager : ConfigManager<KyosekiSetting>
    {
        protected virtual string Filename => "kyoseki.json";

        private readonly Storage storage;

        public KyosekiConfigManager(Storage storage, IDictionary<KyosekiSetting, object> defaultOverrides = null)
            : base(defaultOverrides)
        {
            this.storage = storage;

            InitialiseDefaults();
            Load();
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(KyosekiSetting.Skeletons, Array.Empty<SkeletonSerialProcessorInfo>());
        }

        protected override void PerformLoad()
        {
            using (var stream = storage.GetStream(Filename))
            {
                if (stream == null)
                    return;

                using (var reader = new StreamReader(stream))
                {
                    var config = reader.ReadToEnd();

                    var output = JsonConvert.DeserializeObject<Dictionary<KyosekiSetting, object>>(config);

                    if (output == null) return;

                    foreach (var (key, value) in output)
                    {
                        switch (key)
                        {
                            case KyosekiSetting.Skeletons:
                                var skeletons = JsonConvert.DeserializeObject<SkeletonSerialProcessorInfo[]>(value.ToString());

                                SetValue(KyosekiSetting.Skeletons, skeletons);
                                break;

                            default:
                                if (ConfigStore.TryGetValue(key, out IBindable b))
                                {
                                    try
                                    {
                                        (b as IParseable)?.Parse(value);
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Log($"Failed to parse config key {key}: {e}", LoggingTarget.Runtime, LogLevel.Important);
                                    }
                                }
                                else if (AddMissingEntries)
                                {
                                    SetValue(key, value);
                                }

                                break;
                        }
                    }
                }
            }
        }

        protected override bool PerformSave()
        {
            try
            {
                using (var stream = storage.GetStream(Filename, FileAccess.Write, FileMode.Create))
                using (var w = new StreamWriter(stream))
                {
                    var output = new Dictionary<KyosekiSetting, object>();

                    foreach (var p in ConfigStore)
                        output.Add(p.Key, p.Value);

                    w.Write(JsonConvert.SerializeObject(output));
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    public enum KyosekiSetting
    {
        Skeletons
    }
}
