using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// Records EAN prefix to GS1 Member Organization, where the member organization
    /// correlates strongly with a country. This is an imperfect means of identifying
    /// a country of origin by EAN-13 barcode value. See
    /// <a href="http://en.wikipedia.org/wiki/List_of_GS1_country_codes">
    /// http://en.wikipedia.org/wiki/List_of_GS1_country_codes</a>.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class EANManufacturerOrgSupport
    {

        public EANManufacturerOrgSupport()
        {
            this.ranges = new List<int[]>();
            this.countryIdentifiers = new List<string>();
        }

        private readonly List<int[]> ranges;
        private readonly List<string> countryIdentifiers;

        internal String LookupCountryIdentifier(String productCode)
        {
            InitIfNeeded();
            int prefix = Int32.Parse(productCode.Substring(0, (3) - (0)));
            int max = ranges.Count;
            for (int i = 0; i < max; i++)
            {
                int[] range = (int[])ranges[i];
                int start = range[0];
                if (prefix < start)
                {
                    return null;
                }
                int end = (range.Length == 1) ? start : range[1];
                if (prefix <= end)
                {
                    return countryIdentifiers[i];
                }
            }
            return null;
        }

        private void Add(int[] range, String id)
        {
            ranges.Add(range);
            countryIdentifiers.Add(id);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void InitIfNeeded()
        {
            if (!(ranges.Count == 0))
            {
                return;
            }
            Add(new int[] { 0, 19 }, "US/CA");
            Add(new int[] { 30, 39 }, "US");
            Add(new int[] { 60, 139 }, "US/CA");
            Add(new int[] { 300, 379 }, "FR");
            Add(new int[] { 380 }, "BG");
            Add(new int[] { 383 }, "SI");
            Add(new int[] { 385 }, "HR");
            Add(new int[] { 387 }, "BA");
            Add(new int[] { 400, 440 }, "DE");
            Add(new int[] { 450, 459 }, "JP");
            Add(new int[] { 460, 469 }, "RU");
            Add(new int[] { 471 }, "TW");
            Add(new int[] { 474 }, "EE");
            Add(new int[] { 475 }, "LV");
            Add(new int[] { 476 }, "AZ");
            Add(new int[] { 477 }, "LT");
            Add(new int[] { 478 }, "UZ");
            Add(new int[] { 479 }, "LK");
            Add(new int[] { 480 }, "PH");
            Add(new int[] { 481 }, "BY");
            Add(new int[] { 482 }, "UA");
            Add(new int[] { 484 }, "MD");
            Add(new int[] { 485 }, "AM");
            Add(new int[] { 486 }, "GE");
            Add(new int[] { 487 }, "KZ");
            Add(new int[] { 489 }, "HK");
            Add(new int[] { 490, 499 }, "JP");
            Add(new int[] { 500, 509 }, "GB");
            Add(new int[] { 520 }, "GR");
            Add(new int[] { 528 }, "LB");
            Add(new int[] { 529 }, "CY");
            Add(new int[] { 531 }, "MK");
            Add(new int[] { 535 }, "MT");
            Add(new int[] { 539 }, "IE");
            Add(new int[] { 540, 549 }, "BE/LU");
            Add(new int[] { 560 }, "PT");
            Add(new int[] { 569 }, "IS");
            Add(new int[] { 570, 579 }, "DK");
            Add(new int[] { 590 }, "PL");
            Add(new int[] { 594 }, "RO");
            Add(new int[] { 599 }, "HU");
            Add(new int[] { 600, 601 }, "ZA");
            Add(new int[] { 603 }, "GH");
            Add(new int[] { 608 }, "BH");
            Add(new int[] { 609 }, "MU");
            Add(new int[] { 611 }, "MA");
            Add(new int[] { 613 }, "DZ");
            Add(new int[] { 616 }, "KE");
            Add(new int[] { 618 }, "CI");
            Add(new int[] { 619 }, "TN");
            Add(new int[] { 621 }, "SY");
            Add(new int[] { 622 }, "EG");
            Add(new int[] { 624 }, "LY");
            Add(new int[] { 625 }, "JO");
            Add(new int[] { 626 }, "IR");
            Add(new int[] { 627 }, "KW");
            Add(new int[] { 628 }, "SA");
            Add(new int[] { 629 }, "AE");
            Add(new int[] { 640, 649 }, "FI");
            Add(new int[] { 690, 695 }, "CN");
            Add(new int[] { 700, 709 }, "NO");
            Add(new int[] { 729 }, "IL");
            Add(new int[] { 730, 739 }, "SE");
            Add(new int[] { 740 }, "GT");
            Add(new int[] { 741 }, "SV");
            Add(new int[] { 742 }, "HN");
            Add(new int[] { 743 }, "NI");
            Add(new int[] { 744 }, "CR");
            Add(new int[] { 745 }, "PA");
            Add(new int[] { 746 }, "DO");
            Add(new int[] { 750 }, "MX");
            Add(new int[] { 754, 755 }, "CA");
            Add(new int[] { 759 }, "VE");
            Add(new int[] { 760, 769 }, "CH");
            Add(new int[] { 770 }, "CO");
            Add(new int[] { 773 }, "UY");
            Add(new int[] { 775 }, "PE");
            Add(new int[] { 777 }, "BO");
            Add(new int[] { 779 }, "AR");
            Add(new int[] { 780 }, "CL");
            Add(new int[] { 784 }, "PY");
            Add(new int[] { 785 }, "PE");
            Add(new int[] { 786 }, "EC");
            Add(new int[] { 789, 790 }, "BR");
            Add(new int[] { 800, 839 }, "IT");
            Add(new int[] { 840, 849 }, "ES");
            Add(new int[] { 850 }, "CU");
            Add(new int[] { 858 }, "SK");
            Add(new int[] { 859 }, "CZ");
            Add(new int[] { 860 }, "YU");
            Add(new int[] { 865 }, "MN");
            Add(new int[] { 867 }, "KP");
            Add(new int[] { 868, 869 }, "TR");
            Add(new int[] { 870, 879 }, "NL");
            Add(new int[] { 880 }, "KR");
            Add(new int[] { 885 }, "TH");
            Add(new int[] { 888 }, "SG");
            Add(new int[] { 890 }, "IN");
            Add(new int[] { 893 }, "VN");
            Add(new int[] { 896 }, "PK");
            Add(new int[] { 899 }, "ID");
            Add(new int[] { 900, 919 }, "AT");
            Add(new int[] { 930, 939 }, "AU");
            Add(new int[] { 940, 949 }, "AZ");
            Add(new int[] { 955 }, "MY");
            Add(new int[] { 958 }, "MO");
        }

    }
}
