using System;

namespace NameParser
{
    public class Name : IComparable<Name>
    {
        public Name()
        {
            Salutation = "";
            FirstName = "";
            MiddleInitials = "";
            LastName = "";
            Suffix = "";
        }

        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitials { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }

        public int CompareTo(Name other)
        {
            int diff = CompareLastName(other);
            if (diff != 0)
                return diff;

            diff = CompareFirsName(other);
            if (diff != 0)
                return diff;

            diff = CompareMiddleInitials(other);
            if (diff != 0)
                return diff;

            return 0;
        }

        private int CompareMiddleInitials(Name other) => MiddleInitials.ToLower().CompareTo(other.MiddleInitials.ToLower());

        private int CompareFirsName(Name other) => FirstName.ToLower().CompareTo(other.FirstName.ToLower());

        private int CompareLastName(Name other) => LastName.ToLower().CompareTo(other.LastName.ToLower());

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Name left, Name right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Name left, Name right)
        {
            return !(left == right);
        }

        public static bool operator <(Name left, Name right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Name left, Name right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Name left, Name right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Name left, Name right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
    }
}
