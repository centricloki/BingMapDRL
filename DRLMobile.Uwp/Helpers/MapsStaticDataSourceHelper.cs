using DRLMobile.Core.Interface;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Uwp.Helpers
{
    public class MapsStaticDataSourceHelper : IMapsStaticDataSourceHelper
    {
        /// <summary>
        /// Populated by <c>MapPageViewModel.OnNavigatedToCommandHandler</c> before any legend
        /// is built.  All legend methods that are classification-driven read from this list.
        /// </summary>
        public static List<Classification> ClassificationsList { get; set; } = new List<Classification>();

        // ──────────────────────────────────────────────────────────────────────────
        // Plot-by filter source (static — not classification-driven)
        // ──────────────────────────────────────────────────────────────────────────

        public ObservableCollection<PlotByTypeFilterUIModel> GetPlotByTypeFiltersDataSource()
        {
            ObservableCollection<PlotByTypeFilterUIModel> _plotByFilter = new ObservableCollection<PlotByTypeFilterUIModel>();
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Trade Type",   IsSelected = true,  Tag = Core.Enums.MapFilter.TradeType });
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Account Rank", IsSelected = false, Tag = Core.Enums.MapFilter.Rank });
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Call Date",    IsSelected = false, Tag = Core.Enums.MapFilter.CallDate });
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Cash Sales",   IsSelected = false, Tag = Core.Enums.MapFilter.CashSales });
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Item No",      IsSelected = false, Tag = Core.Enums.MapFilter.Item });
            return _plotByFilter;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Call-date legends  (fixed: Green / Yellow-ish / Orange / Red / Black)
        // These represent time buckets, not classification types — unchanged.
        // ──────────────────────────────────────────────────────────────────────────

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForCallDate()
        {
            ObservableCollection<MapsLegendFilterUIModel> _callDateFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "Less than 1 month", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green),  Tag = 1, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png"    });
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "1-3 months",        IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Yellow), Tag = 2, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Florecent.png" });
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "3-6 months",        IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Orange), Tag = 3, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Yellow.png"   });
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "6 months – 1 year", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Red),    Tag = 4, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Red.png"      });
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "Over 1 year",       IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Black),  Tag = 5, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Black.png"    });
            return _callDateFilter;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Cash-sales legends (fixed: 4 amount tiers — unchanged)
        // ──────────────────────────────────────────────────────────────────────────

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForCashSales()
        {
            ObservableCollection<MapsLegendFilterUIModel> _cashSalesFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = "$0.01 - $100.00",         IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Orange), Tag = 1, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Yellow.png"   });
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = "$100.01 - $500.00",       IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Yellow), Tag = 2, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Florecent.png" });
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = ">$500.01",                IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green),  Tag = 3, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png"    });
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = "$0.00(No Sales Activity)",IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Purple), Tag = 4, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Voilet.png"   });
            return _cashSalesFilter;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Item-no legends (fixed: Sold / Not Sold — unchanged)
        // ──────────────────────────────────────────────────────────────────────────

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForItemNo()
        {
            ObservableCollection<MapsLegendFilterUIModel> _itemNoFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _itemNoFilter.Add(new MapsLegendFilterUIModel() { Title = "Sold",     IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green),  Tag = 1, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png"  });
            _itemNoFilter.Add(new MapsLegendFilterUIModel() { Title = "Not Sold", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Orange), Tag = 2, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Yellow.png" });
            return _itemNoFilter;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Rank legends (fixed: A / B / C / Other — unchanged)
        // ──────────────────────────────────────────────────────────────────────────

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForRank()
        {
            ObservableCollection<MapsLegendFilterUIModel> _rankTypeFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Rank A", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green), Rank = "A", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png" });
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Rank B", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Blue),  Rank = "B", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Blue.png"  });
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Rank C", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Brown), Rank = "C", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Brown.png" });
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Other",  IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Red),   Rank = "",  MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Red.png"   });
            return _rankTypeFilter;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Trade-type legends — DYNAMIC (rebuilt from live DB classifications)
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the Trade Type legend dynamically from <see cref="ClassificationsList"/>.
        ///
        /// <para><b>Wholesale group</b> — all classifications whose <c>CustomerType == 1</c>
        /// plus hard-wired ID 20 are collapsed into a single "Wholesale" legend entry (matching
        /// the original behaviour).</para>
        ///
        /// <para><b>Individual classifications</b> — every other classification gets its own
        /// entry.  Known IDs (22-49) keep their legacy colors and static PNG paths so there
        /// is no visual regression.  New/unknown IDs receive a unique dark color via the
        /// golden-angle HSL algorithm inside <see cref="ClassificationColorService"/>, and
        /// their pin PNG is fetched from the local-storage cache generated during prewarm.</para>
        ///
        /// <para>If <see cref="ClassificationsList"/> is empty (e.g. before first DB load)
        /// the method falls back to the original hard-coded set of known IDs so the map
        /// remains functional.</para>
        /// </summary>
        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForTradeType()
        {
            ObservableCollection<MapsLegendFilterUIModel> tradeTypeFilter =
                new ObservableCollection<MapsLegendFilterUIModel>();

            // ── Fallback: DB not yet loaded ──────────────────────────────────────
            if (ClassificationsList == null || ClassificationsList.Count == 0)
            {
                return BuildFallbackTradeTypeFilter();
            }

            // ── Wholesale group ──────────────────────────────────────────────────
            // CustomerType == 1 classifications + legacy ID 20 (was always in the
            // wholesale bucket in the original code).
            List<int> wholesaleIds = ClassificationsList
                .Where(x => x.CustomerType == 1)
                .Select(a => a.AccountClassificationId)
                .ToList();

            // Ensure ID 20 is included for backward compatibility.
            if (!wholesaleIds.Contains(20))
                wholesaleIds.Add(20);

            if (wholesaleIds.Count > 0)
            {
                int representativeId = wholesaleIds.First();
                tradeTypeFilter.Add(new MapsLegendFilterUIModel
                {
                    Title                  = "Wholesale",
                    IsSelected             = true,
                    BackgroundColor        = ClassificationColorService.GetBrush(representativeId),
                    AccountClassificationIds = wholesaleIds,
                    MapIconImagePath       = ClassificationColorService.GetMapPinPath(representativeId)
                });
            }

            // ── Individual classifications (everything else) ──────────────────────
            var individualClassifications = ClassificationsList
                .Where(x => x.CustomerType != 1 && !wholesaleIds.Contains(x.AccountClassificationId))
                .OrderBy(x => x.AccountClassificationId);

            foreach (var cls in individualClassifications)
            {
                tradeTypeFilter.Add(new MapsLegendFilterUIModel
                {
                    Title                  = cls.AccountClassificationName,
                    IsSelected             = true,
                    BackgroundColor        = ClassificationColorService.GetBrush(cls.AccountClassificationId),
                    AccountClassificationIds = new List<int> { cls.AccountClassificationId },
                    MapIconImagePath       = ClassificationColorService.GetMapPinPath(
                                                cls.AccountClassificationId,
                                                cls.MapPinImageName)
                });
            }

            return tradeTypeFilter;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Private: original hard-coded fallback (used only when ClassificationsList
        //          is empty so the map still renders before the first DB load).
        // ──────────────────────────────────────────────────────────────────────────

        private ObservableCollection<MapsLegendFilterUIModel> BuildFallbackTradeTypeFilter()
        {
            ObservableCollection<MapsLegendFilterUIModel> f = new ObservableCollection<MapsLegendFilterUIModel>();

            List<int> wholesaleIds = new List<int> { 1, 2, 8, 20 };

            f.Add(LegendEntry("MSAi  List A",                    new List<int> { 47 }, 47));
            f.Add(LegendEntry("DM Location",                     new List<int> { 46 }, 46));
            f.Add(LegendEntry("Wholesale",                       wholesaleIds,         1 ));
            f.Add(LegendEntry("C-Store Chain HQ",                new List<int> { 22 }, 22));
            f.Add(LegendEntry("C-Store Chain Location",          new List<int> { 23 }, 23));
            f.Add(LegendEntry("C-Store Independent",             new List<int> { 24 }, 24));
            f.Add(LegendEntry("Tobacco Outlet – Chain HQ",       new List<int> { 25 }, 25));
            f.Add(LegendEntry("Tobacco Outlet – Chain Location", new List<int> { 26 }, 26));
            f.Add(LegendEntry("Tobacco Outlet - Independent",    new List<int> { 27 }, 27));
            f.Add(LegendEntry("Smoke Shop",                      new List<int> { 28 }, 28));
            f.Add(LegendEntry("Dispensary Store",                new List<int> { 29 }, 29));
            f.Add(LegendEntry("S-D-M Chain HQ",                  new List<int> { 30 }, 30));
            f.Add(LegendEntry("S-D-M Chain Location",            new List<int> { 31 }, 31));
            f.Add(LegendEntry("S-D-M – Independent",             new List<int> { 32 }, 32));
            f.Add(LegendEntry("Liquor Store – Chain HQ",         new List<int> { 33 }, 33));
            f.Add(LegendEntry("Liquor Store – Chain Location",   new List<int> { 34 }, 34));
            f.Add(LegendEntry("Liquor Store – Independent",      new List<int> { 35 }, 35));
            f.Add(LegendEntry("Sub Jobber Wholesale",            new List<int> { 36 }, 36));
            f.Add(LegendEntry("Tribal Accounts",                 new List<int> { 37 }, 37));
            f.Add(LegendEntry("Out of business",                 new List<int> { 38 }, 38));
            f.Add(LegendEntry("Smoke Shop - Chain HQ",           new List<int> { 44 }, 44));
            f.Add(LegendEntry("Smoke Shop - Chain Location",     new List<int> { 45 }, 45));
            f.Add(LegendEntry("Manufacturer",                    new List<int> { 48 }, 48));
            //f.Add(LegendEntry("Cultivator",                      new List<int> { 49 }, 49));

            return f;
        }

        /// <summary>
        /// Convenience factory that reads color + pin path from <see cref="ClassificationColorService"/>
        /// so even the fallback list stays DRY and colour-consistent.
        /// </summary>
        private static MapsLegendFilterUIModel LegendEntry(
            string title, List<int> ids, int representativeId)
        {
            return new MapsLegendFilterUIModel
            {
                Title                    = title,
                IsSelected               = true,
                BackgroundColor          = ClassificationColorService.GetBrush(representativeId),
                AccountClassificationIds = ids,
                MapIconImagePath         = ClassificationColorService.GetMapPinPath(representativeId)
            };
        }
    }
}
