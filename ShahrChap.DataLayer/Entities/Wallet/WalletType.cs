using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Wallet;

public class WalletType
{
    public WalletType()
    {
        
    }
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int WalletTypeId { get; set; }
    [Required]
    [MaxLength(250)]
    public string WalletTypeTitle { get; set; }

    #region Relations
    public virtual List<Wallet> Wallets { get; set; }
    #endregion
}