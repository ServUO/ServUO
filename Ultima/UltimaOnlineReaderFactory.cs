using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ultima
{
    
    public sealed class UltimaOnlineReaderFactory : IDisposable
    {
        public enum MapNames
        {
            Felucca,
            Ilshenar,
            Malas,
            Trammel,
            Tokuno,
            TerMur
        }

        private bool disposed;

        private string Folder { get; }
        private Dictionary<MapNames, Map> _Maps;
        public Gumps Gumps { get; }
        public Animations Animations { get; }
        public Skills Skills { get; }
        public Light Light { get; }
        public Multis Multis { get; }
        public Sounds Sound { get; }

        public TileData  TileData { get; }

        public RadarCol RadarCol { get; }

        public Art Art { get; }

        public Files Files { get; }

        public Verdata Verdata { get; }

        public Hues Hues { get; }

        public Textures Textures { get; }

        public AnimationEdit AnimationEdit { get; }

        public ASCIIText ASCIIText { get; }

        public MultiMap MultiMap { get; }

        public Dictionary<string, Map> CustomMaps { get; set; } = new Dictionary<string, Map>();

        public UltimaOnlineReaderFactory(string folder)
        {
            Folder = folder;
            Files = new Files(folder);
            Verdata = new Verdata(this);
            RadarCol = new RadarCol(this);
            Hues = new Hues(this);
            Gumps = new Gumps(this);
            Art = new Art(this);
            TileData = new TileData(this);
            Skills = new Skills(this);
            Light = new Light(this);
            Sound = new Sounds(this);
            Textures = new Textures(this);
            Multis = new Multis(this);
            AnimationEdit = new AnimationEdit(this);
            ASCIIText = new ASCIIText(this);
            _Maps = new Dictionary<MapNames, Map>
            {
                {MapNames.Felucca, Map.Felucca(this)},
                {MapNames.Ilshenar, Map.Ilshenar(this)},
                {MapNames.Malas, Map.Malas(this)},
                {MapNames.Trammel, Map.Trammel(this)},
                {MapNames.Tokuno, Map.Tokuno(this)},
                {MapNames.TerMur, Map.TerMur(this)}
            };
            MultiMap = new Ultima.MultiMap(this);
            Animations = new Animations(Verdata, Hues, Files);
        

        }


        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    foreach (var map in _Maps)
                    {
                        map.Value.Dispose();
                    }

                    if (CustomMaps != null)
                    {
                        foreach (var map in CustomMaps)
                        {
                            map.Value.Dispose();
                        }
                    }
                }


                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                _Maps.Clear();
                _Maps = null;
                // Note disposing has been done.
                disposed = true;

            }
        }



    }
}
