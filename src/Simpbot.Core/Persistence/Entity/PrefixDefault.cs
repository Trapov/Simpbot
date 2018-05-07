using System.ComponentModel.DataAnnotations;

namespace Simpbot.Core.Persistence.Entity
{
    public class PrefixDefault : IPrefix
    {
        [Key]
        public char PrefixId { get; set; }
        public char PrefixSymbol { get; set; }
    }

    public interface IPrefix
    {
        char PrefixSymbol { get; set; }
    }
}