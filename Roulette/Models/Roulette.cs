using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteApp.Models
{
    public class Roulette : BindableBase
    {
        public List<string> Data { get; private set; } = new List<string>();

        private bool isSpin;
        public bool IsSpin
        {
            get { return isSpin; }
            private set { SetProperty(ref this.isSpin, value); }
        }

        public IEnumerable<string> Spin()
        {
            var counta = 0;
            this.IsSpin = true;
            while (this.IsSpin)
            {
                if (Data.Count == counta) counta = 0;
                yield return this.Data.ElementAt(counta);
                counta++;
            }
        }

        public void Stop()
        {
            this.IsSpin = false;
        }
    }
}
