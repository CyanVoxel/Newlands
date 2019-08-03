// A simple struct that holds 2 coordinates as x and y ints.
// @Author: Travis Abendshien (https://github.com/CyanVoxel)

public struct Coordinate2 {

    public int x;
    public int y;

    public Coordinate2(int x, int y) {

        this.x = x;
        this.y = y;

    } // Constructor

    public override bool Equals(object obj) {

        if (obj == null || !this.GetType().Equals(obj.GetType())) {
            return false;
        } else {
            Coordinate2 c = (Coordinate2)obj;
            return (x == c.x && y == c.y);
        }

    } // override Equals()

    public override string ToString() {
        return ("[" + this.x + ", " + this.y + "]");
    } //override ToString()

    public static bool operator ==(Coordinate2 left, Coordinate2 right) {

        if (left.x == right.x && left.y == right.y) {
            return true;
        } else {
            return false;
        }

    } // operator ==

    public static bool operator !=(Coordinate2 left, Coordinate2 right) {

        if (left.x != right.x || left.y != right.y) {
            return true;
        } else {
            return false;
        }

    } // operator !=

    public override int GetHashCode() {

        return x ^ y;

    } // override GetHashCode()

} // Coordinate2 struct
