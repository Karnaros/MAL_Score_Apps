namespace Models
{
    public class GenreComparer : Comparer<Genre>
    {
        public override int Compare(Genre? x, Genre? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.id.CompareTo(y.id);
        }
    }

    public class GenreEqualityComparer : EqualityComparer<Genre>
    {
        public override bool Equals(Genre? x, Genre? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.id == y.id && x.name == y.name;
        }

        public override int GetHashCode(Genre obj)
        {
            int hashId = obj.id.GetHashCode();
            int hashName = obj.name.GetHashCode();

            return hashId ^ hashName;
        }
    }
}
