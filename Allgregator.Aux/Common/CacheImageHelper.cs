using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Allgregator.Aux.Common {
    public static class CacheImageHelper {
        private static Dictionary<Uri, BitmapImage> store = new Dictionary<Uri, BitmapImage>();
        public static BitmapImage Get(Uri key) {
            if (key == null) {
                return null;
            }

            if (store.TryGetValue(key, out BitmapImage resval)) {
                if (resval.PixelHeight == 0 || resval.PixelWidth == 0) {
                    return null;
                }
                else {
                    return resval;
                }
            }

            resval = new BitmapImage(key);
            if (resval == null) {
                return null;
            }

            store.Add(key, resval);
            return resval;
        }
    }
}
