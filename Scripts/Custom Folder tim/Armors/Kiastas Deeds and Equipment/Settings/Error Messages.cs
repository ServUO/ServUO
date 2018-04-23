using System;

namespace Server.Kiasta.Settings
{
    #region Error Messages
    public enum ErrorMessageType
    {
        AttributeError,
        AttributeMax,
        AttributeMaxBool,
        AttributeSuccess,
        AttributeSuccessBool,
        LootTypeError,
        LootTypeExists,
        LootTypeSuccess,
        SlayerNameError,
        SlayerNameExists,
        SlayerNameMax,
        SlayerNameSuccess,
        SlayerNameRemovalError
    }

    public class ErrorMessage
    {
        private ErrorMessageType m_ErrorType;
        private Mobile m_From;
        private string m_AttributeName;
        private object m_Modifier;
        private int m_Max;

        public ErrorMessage(ErrorMessageType errorType, Mobile from, string attributeName, object modifier, int max)
        {
            m_ErrorType = errorType;
            m_From = from;
            m_AttributeName = attributeName;
            m_Modifier = modifier;
            m_Max = max;
            GetMessage();
        }

        private void GetMessage()
        {
            switch (m_ErrorType)
            {
                case ErrorMessageType.AttributeError:
                    {
                        m_From.SendMessage("You cannot apply {0} to that!", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.AttributeMax:
                    {
                        m_From.SendMessage("The item has max {0} already!", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.AttributeMaxBool:
                    {
                        m_From.SendMessage("The item has {0} already!", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.AttributeSuccess:
                    {
                        m_From.SendMessage("You have applied +{0} {1} to that item!", (int)m_Modifier, m_AttributeName);
                        break;
                    }
                case ErrorMessageType.AttributeSuccessBool:
                    {
                        m_From.SendMessage("You have successfully applied {0} to that item!", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.LootTypeError:
                    {
                        m_From.SendMessage("That item cannot be {0}!", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.LootTypeExists:
                    {
                        m_From.SendMessage("That item is already {0}", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.LootTypeSuccess:
                    {
                        m_From.SendMessage("You have {0} that item!", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.SlayerNameError:
                    {
                        m_From.SendMessage("The item cannot have {0}!", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.SlayerNameExists:
                    {
                        m_From.SendMessage("The weapon already has {0}", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.SlayerNameMax:
                    {
                        m_From.SendMessage("You need to apply a 'Slayer Removal Deed' on this item first!");
                        break;
                    }
                case ErrorMessageType.SlayerNameSuccess:
                    {
                        m_From.SendMessage("You have applied {0} to the weapon!", m_AttributeName);
                        break;
                    }
                case ErrorMessageType.SlayerNameRemovalError:
                    {
                        m_From.SendMessage("You do not have slayers on that weapon!");
                        break;
                    }
            }
        }
    }
    #endregion
}
