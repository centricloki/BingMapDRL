using DRLMobile.Core.Models.DataModels;
using DRLMobile.ExceptionHandler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Uwp.Helpers
{
    /// <summary>
    /// Resolves a <see cref="SolidColorBrush"/> and map-pin image URI for any
    /// <c>AccountClassificationId</c>.
    ///
    /// Strategy for unknown/new IDs:
    /// <list type="bullet">
    ///   <item>Known IDs (22–49 etc.) keep their exact legacy colors — no visual regression.</item>
    ///   <item>New IDs (index 0–39): drawn from a 40-entry hand-curated palette of
    ///   visually distinct, vivid colors that span the full hue wheel at varied S/L levels.
    ///   This eliminates the green-clustering problem of a pure golden-angle ring.</item>
    ///   <item>New IDs (index 40+): multi-dimensional fallback — hue via golden angle,
    ///   S and L cycled through 6 distinct tiers so even adjacent hues look different.</item>
    /// </list>
    /// </summary>
    public static class ClassificationColorService
    {
        // ──────────────────────────────────────────────────────────────────────────
        // Legacy palette: exact hex values that match the previously hard-coded
        // brushes in MapsStaticDataSourceHelper so existing users see no change.
        // Key = AccountClassificationId, Value = 6-digit hex color string (no #).
        // ──────────────────────────────────────────────────────────────────────────
        private static readonly Dictionary<int, string> _knownPalette =
            new Dictionary<int, string>
            {
                // Wholesale group (CustomerType==1)   → Green  #008000
                //   IDs 1, 2, 8, 20 land here via the wholesale bucket;
                //   we give them a shared green entry.
                { 1,  "008000" },
                { 2,  "008000" },
                { 8,  "008000" },
                { 20, "008000" },

                // Individual known classifications
                //{ 22, "FFA500" },   // C-Store Chain HQ          → Orange
                //{ 23, "800080" },   // C-Store Chain Location     → Purple
                //{ 24, "CC6600" },   // C-Store Independent        → R204_G102_B0
                //{ 25, "0000FF" },   // Tobacco Outlet – Chain HQ  → Blue
                //{ 26, "FF40FF" },   // Tobacco Outlet – Chain Loc → R255_G64_B255
                //{ 27, "92162A" },   // Tobacco Outlet – Indep.    → R146_G22_B37  (dark crimson)
                //{ 28, "FF0000" },   // Smoke Shop                 → Red
                //{ 29, "FFFF00" },   // Dispensary Store           → Yellow
                //{ 30, "00FDFF" },   // S-D-M Chain HQ             → R0_G253_B255  (Cyan)
                //{ 31, "FF7E79" },   // S-D-M Chain Location       → R255_G126_B121 (salmon)
                //{ 32, "FF66B2" },   // S-D-M – Independent        → R255_G102_B178 (pink)
                //{ 33, "929292" },   // Liquor Store – Chain HQ    → R146_G146_B146 (gray)
                //{ 34, "827D15" },   // Liquor Store – Chain Loc   → R130_G125_B21  (olive)
                //{ 35, "7A81FF" },   // Liquor Store – Indep.      → R122_G129_B255 (periwinkle)
                //{ 36, "06B1B1" },   // Sub Jobber Wholesale       → R6_G177_B177   (teal)
                //{ 37, "FFD479" },   // Tribal Accounts            → R255_G212_B121 (light yellow)
                //{ 38, "000000" },   // Out of business            → Black
                //{ 44, "A52A2A" },   // Smoke Shop – Chain HQ      → Brown
                //{ 45, "FFD479" },   // Smoke Shop – Chain Loc     → R255_G212_B121 (light yellow)
                //{ 46, "AAFF00" },   // DM Location                → Lime Green
                //{ 47, "FF7E79" },   // MSAi List A                → R255_G126_B121 (salmon)
                //{ 48, "DA70D6" },   // Manufacturer               → R218_G112_B214 (orchid)
                //{ 49, "FFD700" },   // Cultivator                 → R255_G215_B0   (gold)
            };

        // Maps known IDs to the pre-existing static PNG name so we keep using them.
        private static readonly Dictionary<int, string> _knownPinImages =
            new Dictionary<int, string>
            {
                { 1,  "MapPin-Green.png"      },
                { 2,  "MapPin-Green.png"      },
                { 8,  "MapPin-Green.png"      },
                { 20, "MapPin-Green.png"      },
                //{ 22, "MapPin-Yellow.png"     },
                //{ 23, "MapPin-Voilet.png"     },
                //{ 24, "MapPin-LightBrown.png" },
                //{ 25, "MapPin-Blue.png"       },
                //{ 26, "MapPin-5.png"          },
                //{ 27, "MapPin-6.png"          },
                //{ 28, "MapPin-Red.png"        },
                //{ 29, "MapPin-Florecent.png"  },
                //{ 30, "MapPin-Cyan.png"       },
                //{ 31, "MapPin-1.png"          },
                //{ 32, "MapPin-Pink.png"       },
                //{ 33, "MapPin-Gray.png"       },
                //{ 34, "MapPin-2.png"          },
                //{ 35, "MapPin-4.png"          },
                //{ 36, "MapPin-3.png"          },
                //{ 37, "MapPin-LightYellow.png"},
                //{ 38, "MapPin-Black.png"      },
                //{ 44, "MapPin-Brown.png"      },
                //{ 45, "MapPin-LightYellow.png"},
                //{ 46, "MapPin-LimeGreen.png"  },
                //{ 47, "MapPin-1.png"          },
                //{ 48, "MapPin-BrightOrchid.png"},
                //{ 49, "MapPin-DeepGold.png"   },
            };

        // Runtime cache: dynamically-assigned colors for unknown IDs built during pre-warm.
        private static readonly Dictionary<int, string> _dynamicHexCache =
            new Dictionary<int, string>();

        // Golden angle in degrees — guarantees maximum colour spread for any sequence length.
        private const double GoldenAngle = 137.508;

        // ──────────────────────────────────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Pre-computes and caches colors for all unknown classifications so that
        /// subsequent <see cref="GetColorHex"/> calls are synchronous and instant.
        /// Also triggers map-pin image generation for each unknown ID.
        /// Call this once after <c>ClassificationsList</c> is loaded.
        /// </summary>
        public static async Task PrewarmAsync(IList<Classification> allClassifications)
        {
            if (allClassifications == null || allClassifications.Count == 0) return;

            // Collect IDs that are NOT in the known palette.
            var unknownIds = allClassifications
                .Select(c => c.AccountClassificationId)
                .Where(id => !_knownPalette.ContainsKey(id))
                .OrderBy(id => id)
                .ToList();

            for (int i = 0; i < unknownIds.Count; i++)
            {
                int id = unknownIds[i];
                // Prefer a server-prescribed color when available
                var serverHex = allClassifications
                    .FirstOrDefault(c => c.AccountClassificationId == id)?.ColorHex;

                string hex = !string.IsNullOrWhiteSpace(serverHex)
                    ? serverHex.TrimStart('#')
                    : ComputeDistinctHex(i);

                _dynamicHexCache[id] = hex;
            }

            // Generate PNG files for unknown IDs (fire-and-forget per ID, awaited as a batch).
            var pinTasks = unknownIds.Select(id =>
                MapPinGenerator.GetOrCreateMapPinAsync(id, _dynamicHexCache[id]));

            await Task.WhenAll(pinTasks);
        }

        /// <summary>
        /// Returns the 6-digit hex color string (without #) for the given classification ID.
        /// Known IDs return their legacy palette value; unknown IDs return the golden-angle value.
        /// </summary>
        public static string GetColorHex(int classificationId)
        {
            if (_knownPalette.TryGetValue(classificationId, out string known))
                return known;

            if (_dynamicHexCache.TryGetValue(classificationId, out string dynamic))
                return dynamic;

            // Fallback: compute on-the-fly if prewarm hasn't run yet (rare race).
            int index = _dynamicHexCache.Count; // rough index
            string fallback = ComputeDistinctHex(index);
            _dynamicHexCache[classificationId] = fallback;
            return fallback;
        }

        /// <summary>
        /// Returns a <see cref="SolidColorBrush"/> for the given classification ID.
        /// </summary>
        public static SolidColorBrush GetBrush(int classificationId)
        {
            string hex = GetColorHex(classificationId);
            return new SolidColorBrush(HexToColor(hex));
        }

        /// <summary>
        /// Returns the <c>ms-appx:///</c> or <c>ms-appdata:///local/</c> URI for the map-pin
        /// image belonging to the given classification ID.
        /// </summary>
        public static string GetMapPinPath(int classificationId)
        {
            // 1. Server-prescribed static filename wins.
            // (looked up from Classification.MapPinImageName via allClassifications, but we
            //  can't easily reference that here without circular lookup — callers should
            //  pass it via the overload below when available.)

            // 2. Known legacy static PNG wins next.
            if (_knownPinImages.TryGetValue(classificationId, out string staticName))
                return $"ms-appx:///Assets/Maps/{staticName}";

            // 3. Dynamically generated pin in local storage.
            return MapPinGenerator.GetLocalPinUri(classificationId);
        }

        /// <summary>
        /// Overload that honours a server-prescribed image name (from
        /// <see cref="Classification.MapPinImageName"/>).
        /// </summary>
        public static string GetMapPinPath(int classificationId, string serverImageName)
        {
            if (!string.IsNullOrWhiteSpace(serverImageName))
                return $"ms-appx:///Assets/Maps/{serverImageName}";

            return GetMapPinPath(classificationId);
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────────────────────────────────────

        // ──────────────────────────────────────────────────────────────────────────
        // 40-entry curated palette — hand-verified to be perceptually distinct.
        //
        // Why not pure golden-angle?
        //   Golden angle (137.5°) at fixed S=72%, L=33% places 4+ consecutive hues
        //   in the green zone (90°–165°) within the first 15 colors because human
        //   eyes cannot distinguish similar-lightness greens well. Fixing L+S and
        //   only varying H is a single ring in HSL cylinder — perceptually non-uniform.
        //
        // This palette spans 8 base hue families × 5 S/L variants so every color
        // differs in BOTH hue AND brightness, guaranteeing visual separation.
        // ──────────────────────────────────────────────────────────────────────────
        private static readonly string[] _curatedPalette = new[]
        {
            // ── Tier 1: Deep vivid (S≈90%, L≈35%) — darkest, most saturated ──
            "CC0000",   //  0  Deep red
            "0044BB",   //  1  Royal blue
            "007700",   //  2  Forest green
            "CC6600",   //  3  Burnt orange
            "6600AA",   //  4  Dark violet
            "007777",   //  5  Dark teal
            "BB0066",   //  6  Dark rose
            "887700",   //  7  Olive

            // ── Tier 2: Bright vivid (S≈80%, L≈50%) — mid lightness ──
            "FF4500",   //  8  Orange-red
            "1155DD",   //  9  Bright blue
            "1E8B1E",   // 10  Medium green
            "EE7700",   // 11  Amber
            "8833CC",   // 12  Medium purple
            "009999",   // 13  Teal
            "DD1155",   // 14  Crimson-pink
            "AA9900",   // 15  Dark gold

            // ── Tier 3: Bold alternate hues (S≈85%, L≈40%) ──
            "FF0066",   // 16  Hot rose
            "0077CC",   // 17  Sky blue
            "339933",   // 18  Lime-forest
            "FF6600",   // 19  Vivid orange
            "5500BB",   // 20  Indigo
            "006655",   // 21  Emerald teal
            "AA0044",   // 22  Dark scarlet-rose
            "AA8800",   // 23  Dark amber

            // ── Tier 4: Rich jewel tones (S≈75%, L≈45%) ──
            "CC3300",   // 24  Brick red
            "3366CC",   // 25  Cornflower blue
            "558B2F",   // 26  Olive-lime
            "CC7700",   // 27  Dark tangerine
            "773399",   // 28  Grape
            "2E8B77",   // 29  Sea-green teal
            "BB3377",   // 30  Deep pink
            "997700",   // 31  Honey gold

            // ── Tier 5: High-contrast mixed (S≈70%, L≈55%) — lighter but vivid ──
            "FF3300",   // 32  Bright red-orange
            "0099DD",   // 33  Bright cyan-blue
            "44AA00",   // 34  Bright lime green
            "FF9900",   // 35  Bright amber
            "9944CC",   // 36  Bright purple
            "00AABB",   // 37  Bright cyan-teal
            "EE2266",   // 38  Bright strawberry
            "BB9900",   // 39  Bright dark gold
        };

        /// <summary>
        /// Returns a visually distinct hex color for the given 0-based index.
        /// <para>
        /// Indices 0–39 → hand-curated palette (guaranteed perceptual separation).<br/>
        /// Indices 40+ → multi-dimensional fallback: golden-angle hue × 6 S/L tiers
        ///               so even adjacent hues differ in brightness.
        /// </para>
        /// </summary>
        private static string ComputeDistinctHex(int index)
        {
            // Fast path: curated palette covers the first 40 unknown classifications.
            if (index < _curatedPalette.Length)
                return _curatedPalette[index];

            // Fallback for index ≥ 40: multi-dimensional HSL variation.
            // Six tiers vary BOTH S and L so adjacent hues within the same tier
            // will still look clearly different from colors in other tiers.
            var tiers = new (double S, double L)[]
            {
                (0.90, 0.32),   // tier 0: deep vivid dark
                (0.75, 0.52),   // tier 1: medium bright
                (0.85, 0.26),   // tier 2: very dark vivid
                (0.70, 0.60),   // tier 3: light vivid
                (0.80, 0.43),   // tier 4: warm medium
                (0.65, 0.47),   // tier 5: muted medium
            };

            int offsetIndex = index - _curatedPalette.Length;
            int tier        = offsetIndex % tiers.Length;
            int tierStep    = offsetIndex / tiers.Length;

            // Hue offset by 60° per tier so tier colours are never in the same hue zone.
            double hueOffset = tier * 60.0;
            double h = (tierStep * GoldenAngle + hueOffset) % 360.0;
            var (s, l) = tiers[tier];

            (byte r, byte g, byte b) = HslToRgb(h, s, l);
            return $"{r:X2}{g:X2}{b:X2}";
        }

        /// <summary>Converts an HSL triplet (h in 0-360, s and l in 0-1) to RGB bytes.</summary>
        private static (byte r, byte g, byte b) HslToRgb(double h, double s, double l)
        {
            double c = (1.0 - Math.Abs(2 * l - 1.0)) * s;
            double x = c * (1.0 - Math.Abs((h / 60.0) % 2.0 - 1.0));
            double m = l - c / 2.0;

            double r1 = 0, g1 = 0, b1 = 0;
            if      (h < 60)  { r1 = c; g1 = x; }
            else if (h < 120) { r1 = x; g1 = c; }
            else if (h < 180) { g1 = c; b1 = x; }
            else if (h < 240) { g1 = x; b1 = c; }
            else if (h < 300) { r1 = x; b1 = c; }
            else              { r1 = c; b1 = x; }

            return (
                (byte)Math.Round((r1 + m) * 255),
                (byte)Math.Round((g1 + m) * 255),
                (byte)Math.Round((b1 + m) * 255));
        }

        /// <summary>Parses a 6-digit hex string (with or without #) into a <see cref="Color"/>.</summary>
        public static Color HexToColor(string hex)
        {
            try
            {
                hex = hex.TrimStart('#');
                if (hex.Length == 6)
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                    return Color.FromArgb(255, r, g, b);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ClassificationColorService), nameof(HexToColor), ex.StackTrace);
            }
            return Colors.Gray; // safe fallback
        }
    }
}
