using System;
using Server;

namespace CustomsFramework.Systems.ShardControl
{
    [PropertyObject]
    public partial class MainSettings
    {
        #region Variables
        private GeneralSettings _GeneralSettings;
        private AccountSettings _AccountSettings;
        private SaveSettings _SaveSettings;
        private ClientSettings _ClientSettings;

        [CommandProperty(AccessLevel.Owner)]
        public GeneralSettings GeneralSettings
        {
            get
            {
                return this._GeneralSettings;
            }
            set
            {
                this._GeneralSettings = value;
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public AccountSettings AccountSettings
        {
            get
            {
                return this._AccountSettings;
            }
            set
            {
                this._AccountSettings = value;
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public SaveSettings SaveSettings
        {
            get
            {
                return this._SaveSettings;
            }
            set
            {
                this._SaveSettings = value;
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public ClientSettings ClientSettings
        {
            get
            {
                return this._ClientSettings;
            }
            set
            {
                this._ClientSettings = value;
            }
        }
        #endregion

        public MainSettings()
        {
            // Finish when gumps are done.
            //this._GeneralSettings = new GeneralSettings();
            //this._AccountSettings = new AccountSettings();
            //this._SaveSettings = new SaveSettings();
            //this._ClientSettings = new ClientSettings();
        }

        public void Serialize(GenericWriter writer)
        {
            Utilities.WriteVersion(writer, 0);

            // Version 0
            this._AccountSettings.Serialize(writer);
            this._SaveSettings.Serialize(writer);
            this._ClientSettings.Serialize(writer);
            this._GeneralSettings.Serialize(writer);
        }

        public MainSettings(GenericReader reader)
        {
            this.Deserialize(reader);
        }

        private void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        this._AccountSettings = new AccountSettings(reader);
                        this._SaveSettings = new SaveSettings(reader);
                        this._ClientSettings = new ClientSettings(reader);
                        this._GeneralSettings = new GeneralSettings(reader);
                        break;
                    }
            }
        }
    }
}