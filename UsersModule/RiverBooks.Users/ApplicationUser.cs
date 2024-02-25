using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    /*
    * Now, whenever you're practicing domain driven design, you want to use encapsulation around you collections. So instead of
    * just exposing a list of cart item right on our user class, instead we're going to wrap that with a read-only collection
    * and reference a private read-only list of cart items, which nobody can set except us, and which we're going to ensure
    * is always instantiated. This will avoid null reference exceptions, and it also makes it so that we're not exposing all
    * of the operations that list of T supports, like clear and remove. We may not want to support that, or if we do want
    * to support it, we probably want to support it directly through our class and not just off of that list property. Case
    * in point, if we just exposed a list, then anybody could call .add on it, and we would have no control over that logic,
    * whereas we definitely want to control the logic of how you add an item to the cart as you're about to see.
    */
    private readonly List<CartItem> _cartItems = new();
    public IReadOnlyCollection<CartItem> CartItems => _cartItems.AsReadOnly();

    /*
    * Now these addresses are going to entities, this is a one-to-many collection, so it's not like the Order where we're
    * just going to add the addresses as complex property, we need to have an actual separate entity for each address,
    * we're going to call that a user street address, and it's just going to have an ID plus the address value object.
    */
    private readonly List<UserStreetAddress> _addresses = [];
    public IReadOnlyCollection<UserStreetAddress> Addresses => _addresses.AsReadOnly();

    public void AddItemToCart(CartItem item)
    {
        Guard.Against.Null(item);

        var existingBook = _cartItems.SingleOrDefault(c => c.BookId == item.BookId);
        if (existingBook is not null)
        {
            existingBook.UpdateQuantity(existingBook.Quantity + item.Quantity);
            existingBook.UpdateDescription(item.Description);
            existingBook.UpdateUnitPrice(item.UnitPrice);
            return;
        }
        _cartItems.Add(item);
    }

    internal UserStreetAddress AddAddress(Address address)
    {
        Guard.Against.Null(address);

        // find existing address and just return it
        var existingAddress = _addresses.SingleOrDefault(a => a.StreetAddress == address);
        if (existingAddress != null)
        {
            return existingAddress;
        }

        var newAddress = new UserStreetAddress(Id, address);
        _addresses.Add(newAddress);

        return newAddress;
    }

    internal void ClearCart()
    {
        _cartItems.Clear();
    }
}
