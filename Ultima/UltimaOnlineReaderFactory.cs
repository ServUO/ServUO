using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ultima
{
    
    public class UltimaOnlineReaderFactory
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

        private String _folder;
        private Verdata _verdata;
        private Files _files;
        private Art _art;
        private TileData _tileData;
        private RadarCol _radarCol;
        private Dictionary<MapNames, Map> maps = new Dictionary<MapNames, Map>();
        private Hues _hues;
        private Gumps _gumps;
        private Animations _animations;
        private Skills _skills;
        private Light _Light;
        private Multis _multis;
        private Sounds _Sound;
        private Textures _Textures;

        public TileData  TileData => _tileData;
        public RadarCol RadarCol => _radarCol;
        public Art Art => _art;
        public Files Files => _files;
        public Verdata Verdata => _verdata;
        public Hues Hues => _hues;

        public UltimaOnlineReaderFactory(String folder)
        {
            _files = new Files(folder);
            _verdata = new Verdata(_files);
            _radarCol = new RadarCol(_files);
            _hues = new Hues(_files);
            _gumps = new Gumps(_verdata, _files);
            _art = new Art(_verdata, _files);
            _tileData = new TileData(_art, _files);
            _skills = new Skills(_verdata, _files);
            _Light = new Light(_verdata, _files);
            _Sound = new Sounds(_verdata, _files);
            _Textures = new Textures(_verdata, _files);
            _multis = new Multis(_verdata, _art, _tileData, _files);
            maps.Add(MapNames.Felucca, Map.Felucca(_radarCol, _tileData, _hues, _files, _art));
            maps.Add(MapNames.Ilshenar, Map.Ilshenar(_radarCol, _tileData, _hues, _files, _art));
            maps.Add(MapNames.Malas, Map.Malas(_radarCol, _tileData, _hues, _files, _art));
            maps.Add(MapNames.Trammel, Map.Trammel(_radarCol, _tileData, _hues, _files, _art));
            maps.Add(MapNames.Tokuno, Map.Tokuno(_radarCol, _tileData, _hues, _files, _art));
            maps.Add(MapNames.TerMur, Map.TerMur(_radarCol, _tileData, _hues, _files, _art));
            _animations = new Animations(_verdata, _hues, _files);
        

        }


    }
}
