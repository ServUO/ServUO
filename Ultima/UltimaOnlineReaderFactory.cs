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

        public string Folder { get; private set;}
        public Dictionary<MapNames, Map> Maps { get; private set; }
        public Gumps Gumps { get; private set; }
        public Animations Animations { get; private set; }
        public Skills Skills { get; private set; }
        public Light Light { get; private set; }
        public Multis Multis { get; private set; }
        public Sounds Sound { get; private set; }

        public TileData  TileData { get; private set; }

        public RadarCol RadarCol { get; private set; }

        public Art Art { get; private set; }

        public Files Files { get; private set; }

        public Verdata Verdata { get; private set; }

        public Hues Hues { get; private set; }

        public Textures Textures { get; private set; }

        public AnimationEdit AnimationEdit { get; private set; }

        public ASCIIText ASCIIText { get; private set; }

        public MultiMap MultiMap { get; private set; }

        public Dictionary<string, Map> CustomMaps { get; set; } = new Dictionary<string, Map>();

        public UltimaOnlineReaderFactory(string folder)
        {
            Folder = folder;
            Files = new Files(this,Folder);

        }

        public void Init()
        {
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
            Maps = new Dictionary<MapNames, Map>
            {
                {MapNames.Felucca, Map.Felucca(this)},
                {MapNames.Ilshenar, Map.Ilshenar(this)},
                {MapNames.Malas, Map.Malas(this)},
                {MapNames.Trammel, Map.Trammel(this)},
                {MapNames.Tokuno, Map.Tokuno(this)},
                {MapNames.TerMur, Map.TerMur(this)}
            };
            MultiMap = new Ultima.MultiMap(this);
            Animations = new Animations(this);
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
                    foreach (var map in Maps)
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
                Maps.Clear();
                
                // Note disposing has been done.
                disposed = true;

            }
        }



    }
}
