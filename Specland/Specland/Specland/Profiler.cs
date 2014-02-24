using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Specland {
    public class Profiler {

        public static Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();

        public static void start(string name) {
            if (!profiles.ContainsKey(name)) {
                profiles.Add(name, new Profile(name));
            }
            profiles[name].start();
        }

        public static void end(string name) {
            if (!profiles.ContainsKey(name)) {
                profiles.Add(name, new Profile(name));
            }
            profiles[name].end();
        }

        public static long get(string name) {
            if (!profiles.ContainsKey(name)) {
                profiles.Add(name, new Profile(name));
            }
            return profiles[name].get();
        }
    }
    public class Profile {
        private string name;

        private Stopwatch stopWatch;
        private long ms = 0;

        public Profile(string name) {
            this.name = name;

            stopWatch = Stopwatch.StartNew();
        }

        public void start() {
            stopWatch.Restart();
        }

        public void end() {
            stopWatch.Stop();
            ms = stopWatch.ElapsedMilliseconds;
        }

        public long get() {
            return ms;
        }
    }
}
