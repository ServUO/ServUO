using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Quests
{
    public class QuestSerializer
    {
        public static object Construct(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
                return null;
            }
        }

        public static QuestSystem DeserializeQuest(GenericReader reader)
        {
            int encoding = reader.ReadEncodedInt();

            switch (encoding)
            {
                default:
                case 0x00: // null
                {
                    return null;
                }
                case 0x01: // old format
                {
                    Type type = ReadQuestType(reader);

                    QuestSystem qs = Construct(type) as QuestSystem;

                    if (qs == null)
                        QuestSystemStub.Instance.BaseDeserialize(reader);
                    else
                        qs.BaseDeserialize(reader);

                    return qs;
                }
                case 0x02:
                {
                    Type type = reader.ReadObjectType();

                    QuestSystem qs = Construct(type) as QuestSystem;

                    Persistence.DeserializeBlock(reader, r => qs?.BaseDeserialize(r));

                    return qs;
                }
            }
        }

        public static void Serialize(QuestSystem qs, GenericWriter writer)
        {
            if (qs == null)
            {
                writer.WriteEncodedInt(0x00);
            }
            else
            {
                writer.WriteEncodedInt(0x02);
                writer.WriteObjectType(qs);

                Persistence.SerializeBlock(writer, qs.BaseSerialize);
            }
        }

        public static QuestObjective DeserializeObjective(QuestSystem qs, GenericReader reader)
        {
            int encoding = reader.ReadEncodedInt();

            switch (encoding)
            {
                default:
                case 0x00: // null
                {
                    return null;
                }
                case 0x01: // old format
                {
                    Type type = ReadType(qs, reader);

                    QuestObjective obj = Construct(type) as QuestObjective;

                    if (obj == null)
                        QuestObjectiveStub.Instance.BaseDeserialize(reader);
                    else
                        obj.BaseDeserialize(reader);

                    return obj;
                }
                case 0x02:
                {
                    Type type = reader.ReadObjectType();

                    QuestObjective obj = Construct(type) as QuestObjective;

                    Persistence.DeserializeBlock(reader, r => obj?.BaseDeserialize(r));

                    return obj;
                }
            }
        }

        public static void Serialize(QuestSystem qs, QuestObjective obj, GenericWriter writer)
        {
            if (obj == null)
            {
                writer.WriteEncodedInt(0x00);
            }
            else
            {
                writer.WriteEncodedInt(0x02);
                writer.WriteObjectType(obj);

                Persistence.SerializeBlock(writer, obj.BaseSerialize);
            }
        }

        public static QuestConversation DeserializeConversation(QuestSystem qs, GenericReader reader)
        {
            int encoding = reader.ReadEncodedInt();

            switch (encoding)
            {
                default:
                case 0x00: // null
                {
                    return null;
                }
                case 0x01: // old format
                {
                    Type type = ReadType(qs, reader);

                    QuestConversation conv = Construct(type) as QuestConversation;

                    if (conv == null)
                        QuestConversationStub.Instance.BaseDeserialize(reader);
                    else
                        conv.BaseDeserialize(reader);

                    return conv;
                }
                case 0x02:
                {
                    Type type = reader.ReadObjectType();

                    QuestConversation conv = Construct(type) as QuestConversation;

                    Persistence.DeserializeBlock(reader, r => conv?.BaseDeserialize(r));

                    return conv;
                }
            }
        }

        public static void Serialize(QuestSystem qs, QuestConversation conv, GenericWriter writer)
        {
            if (conv == null)
            {
                writer.WriteEncodedInt(0x00);
            }
            else
            {
                writer.WriteEncodedInt(0x02);
                writer.WriteObjectType(conv);

                Persistence.SerializeBlock(writer, conv.BaseSerialize);
            }
        }

        #region Old Serialization

        /// <summary>
        /// This dictionary does not need to be updated - not even if any quests change.
        /// It is used strictly for loading the old serialization format.
        /// </summary>
        private static readonly Dictionary<string, string[]> _QuestTable = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Server.Engines.Quests.Doom.TheSummoningQuest"] = new[]
            {
                "AcceptConversation", "CollectBonesObjective", "VanquishDaemonConversation", "VanquishDaemonObjective"
            },

            // Deprecated
            ["Server.Engines.Quests.Necro.DarkTidesQuest"] = new[]
            {
                "AcceptConversation", "AnimateMaabusCorpseObjective", "BankerConversation", "CashBankCheckObjective",
                "FetchAbraxusScrollObjective", "FindBankObjective", "FindCallingScrollObjective", "FindCityOfLightObjective",
                "FindCrystalCaveObjective", "FindMaabusCorpseObjective", "FindMaabusTombObjective", "FindMardothAboutKronusObjective",
                "FindMardothAboutVaultObjective", "FindMardothEndObjective", "FindVaultOfSecretsObjective", "FindWellOfTearsObjective",
                "HorusConversation", "LostCallingScrollConversation", "MaabasConversation", "MardothEndConversation",
                "MardothKronusConversation", "MardothVaultConversation", "RadarConversation", "ReadAbraxusScrollConversation",
                "ReadAbraxusScrollObjective", "ReanimateMaabusConversation", "RetrieveAbraxusScrollObjective", "ReturnToCrystalCaveObjective",
                "SecondHorusConversation", "SpeakCavePasswordObjective", "UseCallingScrollObjective", "VaultOfSecretsConversation",
                "FindHorusAboutRewardObjective", "HealConversation", "HorusRewardConversation"
            },

            // Deprecated
            ["Server.Engines.Quests.Haven.UzeraanTurmoilQuest"] = new[]
            {
                "AcceptConversation", "UzeraanTitheConversation", "UzeraanFirstTaskConversation", "UzeraanReportConversation",
                "SchmendrickConversation", "UzeraanScrollOfPowerConversation", "DryadConversation", "UzeraanFertileDirtConversation",
                "UzeraanDaemonBloodConversation", "UzeraanDaemonBoneConversation", "BankerConversation", "RadarConversation",
                "LostScrollOfPowerConversation", "LostFertileDirtConversation", "DryadAppleConversation", "LostDaemonBloodConversation",
                "LostDaemonBoneConversation", "FindUzeraanBeginObjective", "TitheGoldObjective", "FindUzeraanFirstTaskObjective",
                "KillHordeMinionsObjective", "FindUzeraanAboutReportObjective", "FindSchmendrickObjective", "FindApprenticeObjective",
                "ReturnScrollOfPowerObjective", "FindDryadObjective", "ReturnFertileDirtObjective", "GetDaemonBloodObjective",
                "ReturnDaemonBloodObjective", "GetDaemonBoneObjective", "ReturnDaemonBoneObjective", "CashBankCheckObjective",
                "FewReagentsConversation"
            },

            ["Server.Engines.Quests.Collector.CollectorQuest"] = new[]
            {
                "DontOfferConversation", "DeclineConversation", "AcceptConversation", "ElwoodDuringFishConversation",
                "ReturnPearlsConversation", "AlbertaPaintingConversation", "AlbertaStoolConversation", "AlbertaEndPaintingConversation",
                "AlbertaAfterPaintingConversation", "ElwoodDuringPainting1Conversation", "ElwoodDuringPainting2Conversation", "ReturnPaintingConversation",
                "GabrielAutographConversation", "GabrielNoSheetMusicConversation", "NoSheetMusicConversation", "GetSheetMusicConversation",
                "GabrielSheetMusicConversation", "GabrielIgnoreConversation", "ElwoodDuringAutograph1Conversation", "ElwoodDuringAutograph2Conversation",
                "ElwoodDuringAutograph3Conversation", "ReturnAutographConversation", "TomasToysConversation", "TomasDuringCollectingConversation",
                "ReturnImagesConversation", "ElwoodDuringToys1Conversation", "ElwoodDuringToys2Conversation", "ElwoodDuringToys3Conversation",
                "FullEndConversation", "FishPearlsObjective", "ReturnPearlsObjective", "FindAlbertaObjective",
                "SitOnTheStoolObjective", "ReturnPaintingObjective", "FindGabrielObjective", "FindSheetMusicObjective",
                "ReturnSheetMusicObjective", "ReturnAutographObjective", "FindTomasObjective", "CaptureImagesObjective",
                "ReturnImagesObjective", "ReturnToysObjective", "MakeRoomObjective"
            },

            ["Server.Engines.Quests.Hag.WitchApprenticeQuest"] = new[]
            {
                "FindApprenticeObjective", "FindGrizeldaAboutMurderObjective", "KillImpsObjective", "FindZeefzorpulObjective",
                "ReturnRecipeObjective", "FindIngredientObjective", "ReturnIngredientsObjective", "DontOfferConversation",
                "AcceptConversation", "HagDuringCorpseSearchConversation", "ApprenticeCorpseConversation", "MurderConversation",
                "HagDuringImpSearchConversation", "ImpDeathConversation", "ZeefzorpulConversation", "RecipeConversation",
                "HagDuringIngredientsConversation", "BlackheartFirstConversation", "BlackheartNoPirateConversation", "BlackheartPirateConversation",
                "EndConversation", "RecentlyFinishedConversation"
            },

            ["Server.Engines.Quests.Naturalist.StudyOfSolenQuest"] = new[]
            {
                "StudyNestsObjective", "ReturnToNaturalistObjective", "DontOfferConversation", "AcceptConversation",
                "NaturalistDuringStudyConversation", "EndConversation", "SpecialEndConversation", "FullBackpackConversation"
            },

            ["Server.Engines.Quests.Matriarch.SolenMatriarchQuest"] = new[]
            {
                "DontOfferConversation", "AcceptConversation", "DuringKillInfiltratorsConversation", "GatherWaterConversation",
                "DuringWaterGatheringConversation", "ProcessFungiConversation", "DuringFungiProcessConversation", "FullBackpackConversation",
                "EndConversation", "KillInfiltratorsObjective", "ReturnAfterKillsObjective", "GatherWaterObjective",
                "ReturnAfterWaterObjective", "ProcessFungiObjective", "GetRewardObjective"
            },

            ["Server.Engines.Quests.Ambitious.AmbitiousQueenQuest"] = new[]
            {
                "DontOfferConversation", "AcceptConversation", "DuringKillQueensConversation", "GatherFungiConversation",
                "DuringFungiGatheringConversation", "EndConversation", "FullBackpackConversation", "End2Conversation",
                "KillQueensObjective", "ReturnAfterKillsObjective", "GatherFungiObjective", "GetRewardObjective"
            },

            // Deprecated
            ["Server.Engines.Quests.Ninja.EminosUndertakingQuest"] = new[]
            {
                "AcceptConversation", "FindZoelConversation", "RadarConversation", "EnterCaveConversation",
                "SneakPastGuardiansConversation", "NeedToHideConversation", "UseTeleporterConversation", "GiveZoelNoteConversation",
                "LostNoteConversation", "GainInnInformationConversation", "ReturnFromInnConversation", "SearchForSwordConversation",
                "HallwayWalkConversation", "ReturnSwordConversation", "SlayHenchmenConversation", "ContinueSlayHenchmenConversation",
                "GiveEminoSwordConversation", "LostSwordConversation", "EarnGiftsConversation", "EarnLessGiftsConversation",
                "FindEminoBeginObjective", "FindZoelObjective", "EnterCaveObjective", "SneakPastGuardiansObjective",
                "UseTeleporterObjective", "GiveZoelNoteObjective", "GainInnInformationObjective", "ReturnFromInnObjective",
                "SearchForSwordObjective", "HallwayWalkObjective", "ReturnSwordObjective", "SlayHenchmenObjective",
                "GiveEminoSwordObjective"
            },

            // Deprecated
            ["Server.Engines.Quests.Samurai.HaochisTrialsQuest"] = new[]
            {
                "AcceptConversation", "RadarConversation", "FirstTrialIntroConversation", "FirstTrialKillConversation",
                "GainKarmaConversation", "SecondTrialIntroConversation", "SecondTrialAttackConversation", "ThirdTrialIntroConversation",
                "ThirdTrialKillConversation", "FourthTrialIntroConversation", "FourthTrialCatsConversation", "FifthTrialIntroConversation",
                "FifthTrialReturnConversation", "LostSwordConversation", "SixthTrialIntroConversation", "SeventhTrialIntroConversation",
                "EndConversation", "FindHaochiObjective", "FirstTrialIntroObjective", "FirstTrialKillObjective",
                "FirstTrialReturnObjective", "SecondTrialIntroObjective", "SecondTrialAttackObjective", "SecondTrialReturnObjective",
                "ThirdTrialIntroObjective", "ThirdTrialKillObjective", "ThirdTrialReturnObjective", "FourthTrialIntroObjective",
                "FourthTrialCatsObjective", "FourthTrialReturnObjective", "FifthTrialIntroObjective", "FifthTrialReturnObjective",
                "SixthTrialIntroObjective", "SixthTrialReturnObjective", "SeventhTrialIntroObjective", "SeventhTrialReturnObjective"
            },

            ["Server.Engines.Quests.Zento.TerribleHatchlingsQuest"] = new[]
            {
                "AcceptConversation", "DirectionConversation", "TakeCareConversation", "EndConversation",
                "FirstKillObjective", "SecondKillObjective", "ThirdKillObjective", "ReturnObjective"
            },

            ["Server.Engines.Quests.TimeLord.TimeForLegendsQuest"] = new[]
            {
                "TimeForLegendsObjective"
            },
        };

        private static readonly string[] _QuestTypes = _QuestTable.Keys.ToArray();

        /// <summary>
        /// This method is strictly used for loading the old quest serialization format.
        /// </summary>
        public static Type ReadQuestType(GenericReader reader)
        {
            return ReadType((QuestSystem)null, reader);
        }

        private static Type ReadType(QuestSystem qs, GenericReader reader)
        {
            string[] referenceTable;

            if (qs != null)
            {
                Type type = qs.GetType();

                if (_QuestTable.TryGetValue(type.FullName, out referenceTable) && referenceTable.Length > 0 && !referenceTable[0].Contains('.'))
                {
                    for (int i = 0; i < referenceTable.Length; i++)
                        referenceTable[i] = $"{type.Namespace}.{referenceTable[i]}";
                }
            }
            else
                referenceTable = _QuestTypes;

            return ReadType(referenceTable, reader);
        }

        private static Type ReadType(string[] referenceTable, GenericReader reader)
        {
            int encoding = reader.ReadEncodedInt();

            switch (encoding)
            {
                default:
                case 0x00: // null
                {
                    return null;
                }
                case 0x01: // indexed
                {
                    int index = reader.ReadEncodedInt();

                    if (index >= 0 && index < referenceTable.Length)
                        return ScriptCompiler.FindTypeByFullName(referenceTable[index]);

                    return null;
                }
                case 0x02: // by name
                {
                    string fullName = reader.ReadString();

                    if (fullName == null)
                        return null;

                    return ScriptCompiler.FindTypeByFullName(fullName, false);
                }
            }
        }

        private sealed class QuestSystemStub : QuestSystem
        {
            public static QuestSystemStub Instance => new QuestSystemStub();

            public override object Name { get; } = string.Empty;
            public override object OfferMessage { get; } = 0;
            public override int Picture { get; } = 0;
            public override bool IsTutorial { get; } = false;
            public override TimeSpan RestartDelay { get; } = TimeSpan.Zero;

            private QuestSystemStub()
            { }
        }

        private sealed class QuestObjectiveStub : QuestObjective
        {
            public static QuestObjectiveStub Instance => new QuestObjectiveStub();

            public override object Message { get; } = 0;

            private QuestObjectiveStub()
            { }
        }

        private sealed class QuestConversationStub : QuestConversation
        {
            public static QuestConversationStub Instance => new QuestConversationStub();

            public override object Message { get; } = 0;

            private QuestConversationStub()
            { }
        }

        #endregion
    }
}
