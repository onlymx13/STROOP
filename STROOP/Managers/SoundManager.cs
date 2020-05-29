﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STROOP.Structs;
using System.Windows.Forms;
using STROOP.Utilities;
using STROOP.Controls;
using STROOP.Structs.Configurations;
using System.Drawing;

namespace STROOP.Managers
{
    public class SoundManager
    {
        public SoundManager(TabPage tabPage)
        {
            SplitContainer splitContainerSound = tabPage.Controls["splitContainerSound"] as SplitContainer;

            SplitContainer splitContainerSoundMusic = splitContainerSound.Panel1.Controls["splitContainerSoundMusic"] as SplitContainer;
            ListBox listBoxSoundMusic = splitContainerSoundMusic.Panel1.Controls["listBoxSoundMusic"] as ListBox;
            TextBox textBoxSoundMusic = splitContainerSoundMusic.Panel2.Controls["textBoxSoundMusic"] as TextBox;
            Button buttonSoundPlayMusic = splitContainerSoundMusic.Panel2.Controls["buttonSoundPlayMusic"] as Button;

            SplitContainer splitContainerSoundSoundEffect = splitContainerSound.Panel2.Controls["splitContainerSoundSoundEffect"] as SplitContainer;
            ListBox listBoxSoundSoundEffect = splitContainerSoundSoundEffect.Panel1.Controls["listBoxSoundSoundEffect"] as ListBox;
            TextBox textBoxSoundSoundEffect = splitContainerSoundSoundEffect.Panel2.Controls["textBoxSoundSoundEffect"] as TextBox;
            Button buttonSoundPlaySoundEffect = splitContainerSoundSoundEffect.Panel2.Controls["buttonSoundPlaySoundEffect"] as Button;

            TableConfig.MusicData.GetMusicEntryList().ForEach(musicEntry => listBoxSoundMusic.Items.Add(musicEntry));
            listBoxSoundMusic.Click += (sender, e) =>
            {
                MusicEntry musicEntry = listBoxSoundMusic.SelectedItem as MusicEntry;
                textBoxSoundMusic.Text = musicEntry.Index.ToString();
            };
            buttonSoundPlayMusic.Click += (sender, e) =>
            {
                int? musicIndexNullable = ParsingUtilities.ParseIntNullable(textBoxSoundMusic.Text);
                if (musicIndexNullable == null) return;
                int musicIndex = musicIndexNullable.Value;
                if (musicIndex < 0 || musicIndex > 34) return;
                uint setMusic = RomVersionConfig.SwitchMap(0x80320544, 0x8031F690);
                InGameFunctionCall.WriteInGameFunctionCall(setMusic, 0, (uint)musicIndex, 0);
            };

            foreach (uint soundEffect in _soundEffects)
            {
                string soundEffectString = HexUtilities.FormatValue(soundEffect, 4);
                listBoxSoundSoundEffect.Items.Add(soundEffectString);
            }
            listBoxSoundSoundEffect.Click += (sender, e) =>
            {
                textBoxSoundSoundEffect.Text = listBoxSoundSoundEffect.SelectedItem.ToString() + "FF81";
            };
            buttonSoundPlaySoundEffect.Click += (sender, e) =>
            {
                uint setSound = RomVersionConfig.SwitchMap(0x8031EB00, 0x8031DC78);
                uint soundArg = RomVersionConfig.SwitchMap(0x803331F0, 0x803320E0);
                uint? soundEffectNullable = ParsingUtilities.ParseHexNullable(textBoxSoundSoundEffect.Text);
                if (!soundEffectNullable.HasValue) return;
                uint soundEffect = soundEffectNullable.Value;
                InGameFunctionCall.WriteInGameFunctionCall(setSound, soundEffect, soundArg);
            };
        }

        public void Update(bool updateView)
        {
            if (!updateView) return;
        }

        private List<uint> _soundEffects = new List<uint>()
        {
            0x0400,
            0x0408,
            0x0610,
            0x0418,
            0x0620,
            0x0428,
            0x0429,
            0x042A,
            0x042B,
            0x062C,
            0x042D,
            0x042E,
            0x042F,
            0x0430,
            0x0431,
            0x0432,
            0x0433,
            0x0434,
            0x0435,
            0x0436,
            0x0437,
            0x0438,
            0x043A,
            0x043B,
            0x043C,
            0x043D,
            0x043E,
            0x043F,
            0x0440,
            0x0441,
            0x0442,
            0x0443,
            0x0444,
            0x0444,
            0x0444,
            0x0445,
            0x0446,
            0x0447,
            0x0448,
            0x0450,
            0x0451,
            0x0452,
            0x0456,
            0x0457,
            0x0458,
            0x0459,
            0x045A,
            0x045B,
            0x045C,
            0x045E,
            0x045F,
            0x0460,
            0x1000,
            0x1400,
            0x1001,
            0x1401,
            0x1002,
            0x1402,
            0x1003,
            0x1403,
            0x1004,
            0x1404,
            0x1005,
            0x1405,
            0x1006,
            0x1406,
            0x1007,
            0x1008,
            0x1009,
            0x100A,
            0x100B,
            0x1410,
            0x1411,
            0x1412,
            0x1414,
            0x1416,
            0x1417,
            0x1018,
            0x1019,
            0x1020,
            0x1420,
            0x1021,
            0x1428,
            0x2400,
            0x2401,
            0x2402,
            0x2403,
            0x2404,
            0x2405,
            0x2406,
            0x2407,
            0x2408,
            0x2409,
            0x240A,
            0x240B,
            0x240B,
            0x240C,
            0x240D,
            0x240E,
            0x240F,
            0x2410,
            0x2411,
            0x2411,
            0x2412,
            0x2413,
            0x2413,
            0x2414,
            0x2415,
            0x2416,
            0x2417,
            0x2418,
            0x2419,
            0x241A,
            0x241B,
            0x241C,
            0x241D,
            0x241E,
            0x241F,
            0x2420,
            0x2421,
            0x2422,
            0x2423,
            0x2424,
            0x2425,
            0x2426,
            0x2427,
            0x2428,
            0x2429,
            0x242A,
            0x242B,
            0x242C,
            0x242D,
            0x242E,
            0x242F,
            0x2430,
            0x2431,
            0x2432,
            0x2433,
            0x2434,
            0x2435,
            0x2436,
            0x2437,
            0x2438,
            0x2439,
            0x243A,
            0x243B,
            0x243C,
            0x243D,
            0x243E,
            0x243F,
            0x3000,
            0x3001,
            0x3002,
            0x3003,
            0x3004,
            0x3005,
            0x3006,
            0x3007,
            0x3008,
            0x3009,
            0x300A,
            0x300B,
            0x300C,
            0x300D,
            0x300E,
            0x300F,
            0x3010,
            0x3811,
            0x3812,
            0x3013,
            0x3014,
            0x3015,
            0x3016,
            0x3017,
            0x3018,
            0x3019,
            0x301A,
            0x301B,
            0x301C,
            0x301D,
            0x301E,
            0x301F,
            0x3120,
            0x3021,
            0x3122,
            0x3023,
            0x3024,
            0x3224,
            0x3025,
            0x3225,
            0x3026,
            0x3027,
            0x3028,
            0x3828,
            0x3928,
            0x3029,
            0x302A,
            0x302B,
            0x302C,
            0x302D,
            0x302E,
            0x312F,
            0x3030,
            0x3830,
            0x3031,
            0x3032,
            0x3033,
            0x3034,
            0x3035,
            0x3036,
            0x3037,
            0x3037,
            0x3837,
            0x3038,
            0x3039,
            0x303A,
            0x303B,
            0x303C,
            0x303D,
            0x303D,
            0x303E,
            0x303F,
            0x3040,
            0x3040,
            0x3041,
            0x3042,
            0x3043,
            0x3044,
            0x3045,
            0x3046,
            0x3046,
            0x3047,
            0x3048,
            0x3049,
            0x304A,
            0x304B,
            0x304C,
            0x314D,
            0x304E,
            0x304F,
            0x3050,
            0x3051,
            0x3052,
            0x3053,
            0x3054,
            0x3055,
            0x3056,
            0x3057,
            0x3058,
            0x3059,
            0x305A,
            0x305A,
            0x315A,
            0x315A,
            0x305B,
            0x315B,
            0x305C,
            0x315C,
            0x305D,
            0x305E,
            0x305F,
            0x3060,
            0x3061,
            0x3062,
            0x3162,
            0x3063,
            0x3064,
            0x3065,
            0x3066,
            0x3067,
            0x3068,
            0x3069,
            0x306A,
            0x306B,
            0x306C,
            0x306D,
            0x306D,
            0x306E,
            0x306F,
            0x3070,
            0x3071,
            0x3072,
            0x3073,
            0x3074,
            0x3075,
            0x3076,
            0x3077,
            0x3078,
            0x3079,
            0x307A,
            0x307B,
            0x307C,
            0x307D,
            0x307E,
            0x307F,
            0x4000,
            0x4001,
            0x4002,
            0x4103,
            0x4004,
            0x4005,
            0x4006,
            0x4007,
            0x4008,
            0x4009,
            0x400A,
            0x400B,
            0x400C,
            0x400D,
            0x410D,
            0x400E,
            0x400F,
            0x4010,
            0x4011,
            0x4012,
            0x4013,
            0x4014,
            0x4115,
            0x4116,
            0x4017,
            0x4018,
            0x4019,
            0x401A,
            0x401B,
            0x5000,
            0x5001,
            0x5002,
            0x5003,
            0x5004,
            0x5005,
            0x5006,
            0x5007,
            0x5008,
            0x5009,
            0x500A,
            0x500B,
            0x500C,
            0x500D,
            0x500E,
            0x500F,
            0x5010,
            0x5011,
            0x5012,
            0x5013,
            0x5014,
            0x5015,
            0x5015,
            0x5016,
            0x5016,
            0x5017,
            0x5018,
            0x5118,
            0x5019,
            0x501A,
            0x501B,
            0x501C,
            0x501D,
            0x501E,
            0x501F,
            0x5020,
            0x5021,
            0x5022,
            0x5022,
            0x5023,
            0x5024,
            0x5025,
            0x5026,
            0x5027,
            0x5028,
            0x5029,
            0x502A,
            0x502B,
            0x502C,
            0x502C,
            0x502D,
            0x502E,
            0x502F,
            0x502F,
            0x5030,
            0x5031,
            0x5032,
            0x5033,
            0x5034,
            0x5034,
            0x5035,
            0x5035,
            0x5036,
            0x5037,
            0x5038,
            0x5039,
            0x503A,
            0x503B,
            0x503C,
            0x503D,
            0x503E,
            0x503F,
            0x5040,
            0x5041,
            0x5042,
            0x5043,
            0x5044,
            0x5045,
            0x5046,
            0x5147,
            0x5048,
            0x5049,
            0x504A,
            0x524A,
            0x524B,
            0x504C,
            0x504D,
            0x504E,
            0x504F,
            0x5050,
            0x5051,
            0x5052,
            0x5053,
            0x5054,
            0x5055,
            0x5256,
            0x5057,
            0x5058,
            0x5059,
            0x505A,
            0x505B,
            0x505C,
            0x505D,
            0x505E,
            0x505F,
            0x5060,
            0x5061,
            0x5062,
            0x5063,
            0x5063,
            0x5063,
            0x5064,
            0x5065,
            0x5066,
            0x5067,
            0x5068,
            0x5069,
            0x506A,
            0x506B,
            0x506C,
            0x506D,
            0x516E,
            0x506F,
            0x5070,
            0x5071,
            0x5072,
            0x5073,
            0x5074,
            0x5075,
            0x5076,
            0x5077,
            0x5078,
            0x5079,
            0x507A,
            0x507B,
            0x507C,
            0x507D,
            0x507E,
            0x507F,
            0x6000,
            0x6002,
            0x6002,
            0x6003,
            0x6004,
            0x6004,
            0x6005,
            0x6006,
            0x6008,
            0x6009,
            0x600A,
            0x600B,
            0x6010,
            0x7000,
            0x7001,
            0x7002,
            0x7002,
            0x7003,
            0x7004,
            0x7005,
            0x7006,
            0x7007,
            0x7008,
            0x7009,
            0x700A,
            0x700B,
            0x700C,
            0x700D,
            0x700E,
            0x700F,
            0x7010,
            0x7011,
            0x7012,
            0x7013,
            0x7014,
            0x7015,
            0x7016,
            0x7017,
            0x7018,
            0x7119,
            0x701A,
            0x701B,
            0x701C,
            0x701D,
            0x701E,
            0x701F,
            0x7020,
            0x7021,
            0x7022,
            0x7023,
            0x7024,
            0x7828,
            0x7030,
            0x802E,
            0x803E,
            0x8040,
            0x8048,
            0x814B,
            0x814C,
            0x8050,
            0x8054,
            0x8055,
            0x8057,
            0x8059,
            0x8060,
            0x8061,
            0x8063,
            0x806A,
            0x9004,
            0x9010,
            0x9011,
            0x9019,
            0x901C,
            0x9142,
            0x9043,
            0x9044,
            0x9045,
            0x9049,
            0x9052,
            0x9057,
            0x935A,
            0x935A,
            0x925B,
            0x9066,
            0x9067,
            0x9069,
            0x906B,
        };
    }
}
