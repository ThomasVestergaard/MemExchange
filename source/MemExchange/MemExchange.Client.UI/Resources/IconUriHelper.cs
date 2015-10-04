using System;

namespace MemExchange.Client.UI.Resources
{
    public class IconUriHelper
    {
        private static string icons16x16BasePath = "pack://application:,,,/MemExchange.Client.UI;component/Resources/Icons/16x16/";

        public static Uri accept_16x16 { get { return new Uri(icons16x16BasePath + "accept.png", UriKind.RelativeOrAbsolute); } }
        public static Uri add_16x16 { get { return new Uri(icons16x16BasePath + "add.png", UriKind.RelativeOrAbsolute); } }
        public static Uri arrow_refresh_16x16 { get { return new Uri(icons16x16BasePath + "arrow_refresh.png", UriKind.RelativeOrAbsolute); } }
        public static Uri cancel_16x16 { get { return new Uri(icons16x16BasePath + "cancel.png", UriKind.RelativeOrAbsolute); } }
        public static Uri folder_database_16x16 { get { return new Uri(icons16x16BasePath + "folder_database.png", UriKind.RelativeOrAbsolute); } }
        public static Uri pencil_16x16 { get { return new Uri(icons16x16BasePath + "pencil.png", UriKind.RelativeOrAbsolute); } }
        public static Uri find_16x16 { get { return new Uri(icons16x16BasePath + "find.png", UriKind.RelativeOrAbsolute); } }
        public static Uri chart_curve_16x16 { get { return new Uri(icons16x16BasePath + "chart_curve.png", UriKind.RelativeOrAbsolute); } }
        public static Uri chart_organisation_16x16 { get { return new Uri(icons16x16BasePath + "chart_organisation.png", UriKind.RelativeOrAbsolute); } }
        public static Uri cog_16x16 { get { return new Uri(icons16x16BasePath + "16x16/cog.png", UriKind.RelativeOrAbsolute); } }
        public static Uri cross_16x16 { get { return new Uri(icons16x16BasePath + "cross.png", UriKind.RelativeOrAbsolute); } }
        public static Uri wrench_16x16 { get { return new Uri(icons16x16BasePath + "wrench.png", UriKind.RelativeOrAbsolute); } }
        public static Uri money_dollar_16x16 { get { return new Uri(icons16x16BasePath + "money_dollar.png", UriKind.RelativeOrAbsolute); } }
        public static Uri book_open_16x16 { get { return new Uri(icons16x16BasePath + "book_open.png", UriKind.RelativeOrAbsolute); } }
        public static Uri trade_16x16 { get { return new Uri(icons16x16BasePath + "trade.png", UriKind.RelativeOrAbsolute); } }
    }
}
