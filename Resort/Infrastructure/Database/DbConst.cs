namespace Resort.Infrastructure.Database;

public static class DbConst
{
    public static class Place
    {
        public const int MaxNameLength = 200;
        public const int MaxCityLength = 100;
        public const int MaxRegionLength = 100;
        public const int MaxAddressLength = 250;
        public const int MaxDescriptionLength = 2000;
        public const int MaxGoogleMapUrlLength = 500;
    }

    public static class Feature
    {
        public const int MaxNameLength = 100;
        public const int MaxIconKeyLength = 50;
    }

    public static class User
    {
        public const int MaxIdLength = 64;
        public const int MaxFullNameLength = 150;
        public const int MaxEmailLength = 150;
    }

    public static class Booking
    {
        public const int MaxUserIdLength = 64;

        public const int PricePrecision = 18;
        public const int PriceScale = 2;
    }
}
