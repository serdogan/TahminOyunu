using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cstech.Models
{
    /// <summary>
    /// DataGridViewRow'a kullanıcının tahminlerini yazmak için kullanılıyor
    /// </summary>
    public class Tahminler
    {
        [Description("Tahmin Sırası")]
        public int TahminSirasi { get; set; }

        [Description("Değer")]
        public int Deger { get; set; }
    }
}
