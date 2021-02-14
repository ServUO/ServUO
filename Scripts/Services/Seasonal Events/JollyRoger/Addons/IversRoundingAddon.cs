
namespace Server.Items
{
    public class IversRoundingAddon : BaseAddon
    {
        public static IversRoundingAddon InstanceTram { get; set; }
        public static IversRoundingAddon InstanceFel { get; set; }

        private static readonly int[,] m_AddOnSimpleComponents =
        {
              {8600, 7, 7, 6}, {6057, 1, 6, 0}, {6057, 0, 6, 0}	
			, {6057, -1, 6, 0}, {6060, 3, 5, 0}, {6057, 2, 5, 0}	
			, {8778, 7, 3, 5}, {3329, 8, 2, 5}, {4970, 8, 1, 5}	
			, {4970, 7, 1, 5}, {3480, 9, -3, 5}, {3280, 7, -3, 5}
			, {3277, 6, 4, 5}, {6059, 3, 4, 0}, {6058, 2, 4, 0}
			, {6059, 4, 3, 0}, {3293, 6, 2, 5}, {6059, 4, 2, 0}
			, {4970, 6, 1, 5}, {6059, 4, 1, 0}, {8611, -1, 1, 25}
			, {6059, 4, 0, 0}, {8611, 0, 0, 40}, {8611, -1, 0, 25}
			, {6059, 4, -1, 0}, {8611, 0, -1, 40}, {8611, -1, -1, 25}
			, {6059, 4, -2, 0}, {8611, 0, -2, 25}, {8611, -1, -2, 25}
			, {3329, 6, -3, 5}, {3326, 5, -3, 4}, {6059, 4, -3, 0}
			, {8611, 0, -3, 25}, {8611, -1, -3, 25}, {3293, 8, -4, 5}
			, {6059, 4, -4, 0}, {8611, -1, -4, 25}, {6059, 4, -5, 0}
			, {6057, -2, 6, 0}, {6057, -3, 6, 0}, {6057, -4, 6, 0}
			, {6057, -5, 6, 0}, {6057, -6, 6, 0}, {6057, -7, 6, 0}
			, {8611, -2, 2, 25}, {8611, -3, 2, 25}, {8611, -4, 2, 25}
			, {8611, -5, 2, 25}, {8611, -2, 1, 25}, {8611, -3, 1, 25}
			, {8611, -4, 1, 25}, {8611, -5, 1, 25}, {8611, -6, 1, 25}
			, {8611, -2, 0, 25}, {8611, -3, 0, 25}, {8611, -4, 0, 25}
			, {8611, -5, 0, 25}, {8611, -6, 0, 25}, {8611, -2, -1, 25}
			, {8611, -3, -1, 25}, {8611, -4, -1, 25}, {8611, -5, -1, 25}
			, {8611, -6, -1, 25}, {8611, -2, -2, 25}, {8611, -3, -2, 25}
			, {8611, -4, -2, 25}, {8611, -5, -2, 25}, {8611, -6, -2, 25}
			, {8611, -2, -3, 25}, {8611, -3, -3, 25}, {8611, -4, -3, 25}
			, {8611, -5, -3, 25}, {8611, -2, -4, 25}, {8611, -3, -4, 25}
			, {8611, -4, -4, 25}, {14217, 1, 0, 40}
        };

        [Constructable]
        public IversRoundingAddon()
        {

            for (var i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 211, 1, 6, 0, 1885, -1, "", 1);// 2
            AddComplexComponent(this, 211, 0, 6, 0, 1884, -1, "", 1);// 4
            AddComplexComponent(this, 211, -1, 6, 0, 1884, -1, "", 1);// 6
            AddComplexComponent(this, 211, 2, 5, 0, 1887, -1, "", 1);// 9
            AddComplexComponent(this, 210, 2, 5, 0, 1885, -1, "", 1);// 10
            AddComplexComponent(this, 204, 1, 5, 0, 1885, -1, "", 1);// 12
            AddComplexComponent(this, 215, 1, 5, 20, 1885, -1, "", 1);// 13
            AddComplexComponent(this, 204, 0, 5, 0, 1884, -1, "", 1);// 14
            AddComplexComponent(this, 215, 0, 5, 20, 1884, -1, "", 1);// 15
            AddComplexComponent(this, 197, -1, 5, 0, 1884, -1, "", 1);// 16
            AddComplexComponent(this, 197, -1, 5, 20, 1884, -1, "", 1);// 17
            AddComplexComponent(this, 1279, -1, 5, 40, 1884, -1, "", 1);// 18
            AddComplexComponent(this, 220, -1, 5, 40, 1884, -1, "", 1);// 19
            AddComplexComponent(this, 219, -1, 5, 44, 1884, -1, "", 1);// 20
            AddComplexComponent(this, 1846, 10, 0, 5, 1890, -1, "", 1);// 25
            AddComplexComponent(this, 1846, 9, 0, 10, 1890, -1, "", 1);// 26
            AddComplexComponent(this, 223, 9, 0, 5, 1890, -1, "", 1);// 27
            AddComplexComponent(this, 1846, 8, 0, 15, 1890, -1, "", 1);// 28
            AddComplexComponent(this, 223, 8, 0, 5, 1890, -1, "", 1);// 29
            AddComplexComponent(this, 223, 8, 0, 8, 1890, -1, "", 1);// 30
            AddComplexComponent(this, 223, 8, 0, 11, 1890, -1, "", 1);// 31
            AddComplexComponent(this, 1846, 7, 0, 20, 1890, -1, "", 1);// 32
            AddComplexComponent(this, 223, 7, 0, 5, 1890, -1, "", 1);// 33
            AddComplexComponent(this, 223, 7, 0, 8, 1890, -1, "", 1);// 34
            AddComplexComponent(this, 223, 7, 0, 11, 1890, -1, "", 1);// 35
            AddComplexComponent(this, 223, 7, 0, 14, 1890, -1, "", 1);// 36
            AddComplexComponent(this, 223, 7, 0, 17, 1890, -1, "", 1);// 37
            AddComplexComponent(this, 1846, 10, -1, 5, 1890, -1, "", 1);// 38
            AddComplexComponent(this, 1846, 9, -1, 10, 1890, -1, "", 1);// 39
            AddComplexComponent(this, 1846, 8, -1, 15, 1890, -1, "", 1);// 40
            AddComplexComponent(this, 1846, 7, -1, 20, 1890, -1, "", 1);// 41
            AddComplexComponent(this, 4962, 7, -2, 5, 922, -1, "", 1);// 42
            AddComplexComponent(this, 211, 3, 4, 0, 1888, -1, "", 1);// 46
            AddComplexComponent(this, 210, 3, 4, 0, 1887, -1, "", 1);// 47
            AddComplexComponent(this, 204, 2, 4, 0, 1887, -1, "", 1);// 49
            AddComplexComponent(this, 212, 2, 4, 20, 1887, -1, "", 1);// 50
            AddComplexComponent(this, 197, 1, 4, 0, 1886, -1, "", 1);// 52
            AddComplexComponent(this, 197, 1, 4, 20, 1886, -1, "", 1);// 53
            AddComplexComponent(this, 1279, 1, 4, 40, 1886, -1, "", 1);// 54
            AddComplexComponent(this, 220, 1, 4, 40, 1886, -1, "", 1);// 55
            AddComplexComponent(this, 219, 1, 4, 44, 1886, -1, "", 1);// 56
            AddComplexComponent(this, 197, 0, 4, 0, 1885, -1, "", 1);// 57
            AddComplexComponent(this, 197, 0, 4, 20, 1885, -1, "", 1);// 58
            AddComplexComponent(this, 1278, 0, 4, 40, 1885, -1, "", 1);// 59
            AddComplexComponent(this, 197, -1, 4, 0, 1883, -1, "", 1);// 60
            AddComplexComponent(this, 197, -1, 4, 20, 1882, -1, "", 1);// 61
            AddComplexComponent(this, 1276, -1, 4, 40, 1884, -1, "", 1);// 62
            AddComplexComponent(this, 210, 4, 3, 0, 1888, -1, "", 1);// 63
            AddComplexComponent(this, 204, 3, 3, 0, 1888, -1, "", 1);// 65
            AddComplexComponent(this, 216, 3, 3, 20, 1888, -1, "", 1);// 66
            AddComplexComponent(this, 197, 2, 3, 0, 1888, -1, "", 1);// 67
            AddComplexComponent(this, 197, 2, 3, 20, 1888, -1, "", 1);// 68
            AddComplexComponent(this, 1279, 2, 3, 40, 1888, -1, "", 1);// 69
            AddComplexComponent(this, 220, 2, 3, 40, 1888, -1, "", 1);// 70
            AddComplexComponent(this, 219, 2, 3, 44, 1888, -1, "", 1);// 71
            AddComplexComponent(this, 197, 1, 3, 0, 1885, -1, "", 1);// 72
            AddComplexComponent(this, 197, 1, 3, 20, 1885, -1, "", 1);// 73
            AddComplexComponent(this, 1278, 1, 3, 40, 1887, -1, "", 1);// 74
            AddComplexComponent(this, 197, 0, 3, 0, 1884, -1, "", 1);// 75
            AddComplexComponent(this, 197, 0, 3, 20, 1884, -1, "", 1);// 76
            AddComplexComponent(this, 1277, 0, 3, 40, 1886, -1, "", 1);// 77
            AddComplexComponent(this, 210, 4, 2, 0, 1889, -1, "", 1);// 79
            AddComplexComponent(this, 204, 3, 2, 0, 1889, -1, "", 1);// 81
            AddComplexComponent(this, 216, 3, 2, 20, 1889, -1, "", 1);// 82
            AddComplexComponent(this, 197, 2, 2, 0, 1889, -1, "", 1);// 83
            AddComplexComponent(this, 197, 2, 2, 20, 1889, -1, "", 1);// 84
            AddComplexComponent(this, 1276, 2, 2, 40, 1889, -1, "", 1);// 85
            AddComplexComponent(this, 197, 1, 2, 0, 1886, -1, "", 1);// 86
            AddComplexComponent(this, 197, 1, 2, 20, 1886, -1, "", 1);// 87
            AddComplexComponent(this, 1277, 1, 2, 40, 1888, -1, "", 1);// 88
            AddComplexComponent(this, 210, 4, 1, 0, 1890, -1, "", 1);// 90
            AddComplexComponent(this, 197, 3, 1, 0, 1890, -1, "", 1);// 92
            AddComplexComponent(this, 197, 3, 1, 20, 1890, -1, "", 1);// 93
            AddComplexComponent(this, 1279, 3, 1, 40, 1890, -1, "", 1);// 94
            AddComplexComponent(this, 220, 3, 1, 40, 1890, -1, "", 1);// 95
            AddComplexComponent(this, 219, 3, 1, 44, 1890, -1, "", 1);// 96
            AddComplexComponent(this, 197, 2, 1, 0, 1890, -1, "", 1);// 97
            AddComplexComponent(this, 197, 2, 1, 20, 1890, -1, "", 1);// 98
            AddComplexComponent(this, 1276, 2, 1, 40, 1889, -1, "", 1);// 99
            AddComplexComponent(this, 4990, 1, 1, 36, 1266, -1, "", 1);// 100
            AddComplexComponent(this, 4990, -1, 1, 20, 1266, -1, "", 1);// 102
            AddComplexComponent(this, 1846, 6, 0, 25, 1890, -1, "", 1);// 103
            AddComplexComponent(this, 471, 6, 0, 5, 1890, -1, "", 1);
            AddComplexComponent(this, 1846, 5, 0, 30, 1890, -1, "", 1);
            AddComplexComponent(this, 204, 5, 0, 5, 1890, -1, "", 1);// 106
            AddComplexComponent(this, 207, 5, 0, 10, 1890, -1, "", 1);// 107
            AddComplexComponent(this, 210, 4, 0, 0, 1890, -1, "", 1);// 108
            AddComplexComponent(this, 1846, 4, 0, 35, 1890, -1, "", 1);// 110
            AddComplexComponent(this, 197, 3, 0, 0, 1890, -1, "", 1);// 111
            AddComplexComponent(this, 197, 3, 0, 20, 1890, -1, "", 1);// 112
            AddComplexComponent(this, 1278, 3, 0, 40, 1890, -1, "", 1);// 113
            AddComplexComponent(this, 197, 2, 0, 0, 1890, -1, "", 1);// 114
            AddComplexComponent(this, 197, 2, 0, 20, 1890, -1, "", 1);// 115
            AddComplexComponent(this, 1277, 2, 0, 40, 1890, -1, "", 1);// 116
            AddComplexComponent(this, 1278, 1, 0, 40, 1890, -1, "", 1);// 117
            AddComplexComponent(this, 200, 1, 0, 20, 1890, -1, "", 1);// 118
            AddComplexComponent(this, 1278, 0, 0, 20, 1, -1, "", 1);// 120
            AddComplexComponent(this, 1278, 0, 0, 25, 1, -1, "", 1);// 121
            AddComplexComponent(this, 4990, -1, 0, 20, 1266, -1, "", 1);// 123
            AddComplexComponent(this, 1846, 6, -1, 25, 1890, -1, "", 1);// 124
            AddComplexComponent(this, 207, 6, -1, 5, 1890, -1, "", 1);// 125
            AddComplexComponent(this, 207, 5, -1, 5, 1890, -1, "", 1);// 126
            AddComplexComponent(this, 1846, 5, -1, 30, 1890, -1, "", 1);// 127
            AddComplexComponent(this, 210, 4, -1, 0, 1890, -1, "", 1);// 128
            AddComplexComponent(this, 1846, 4, -1, 35, 1890, -1, "", 1);// 130
            AddComplexComponent(this, 197, 3, -1, 0, 1890, -1, "", 1);// 131
            AddComplexComponent(this, 197, 3, -1, 20, 1890, -1, "", 1);// 132
            AddComplexComponent(this, 1277, 3, -1, 40, 1890, -1, "", 1);// 133
            AddComplexComponent(this, 197, 2, -1, 0, 1890, -1, "", 1);// 134
            AddComplexComponent(this, 197, 2, -1, 20, 1890, -1, "", 1);// 135
            AddComplexComponent(this, 1278, 2, -1, 40, 1890, -1, "", 1);// 136
            AddComplexComponent(this, 1277, 1, -1, 40, 1890, -1, "", 1);// 137
            AddComplexComponent(this, 1278, 0, -1, 20, 1, -1, "", 1);// 139
            AddComplexComponent(this, 1278, 0, -1, 25, 1, -1, "", 1);// 140
            AddComplexComponent(this, 4990, -1, -1, 20, 1266, -1, "", 1);// 142
            AddComplexComponent(this, 4962, 6, -2, 5, 922, -1, "", 1);// 143
            AddComplexComponent(this, 4962, 5, -2, 5, 922, -1, "", 1);// 144
            AddComplexComponent(this, 210, 4, -2, 0, 1890, -1, "", 1);// 145
            AddComplexComponent(this, 197, 3, -2, 0, 1890, -1, "", 1);// 147
            AddComplexComponent(this, 197, 3, -2, 20, 1890, -1, "", 1);// 148
            AddComplexComponent(this, 1276, 3, -2, 40, 1890, -1, "", 1);// 149
            AddComplexComponent(this, 221, 3, -2, 40, 1890, -1, "", 1);// 150
            AddComplexComponent(this, 197, 2, -2, 0, 1890, -1, "", 1);// 151
            AddComplexComponent(this, 197, 2, -2, 20, 1890, -1, "", 1);// 152
            AddComplexComponent(this, 1279, 2, -2, 40, 1890, -1, "", 1);// 153
            AddComplexComponent(this, 4990, 0, -2, 20, 1266, -1, "", 1);// 155
            AddComplexComponent(this, 4990, -1, -2, 20, 1266, -1, "", 1);// 157
            AddComplexComponent(this, 210, 4, -3, 0, 1890, -1, "", 1);// 160
            AddComplexComponent(this, 204, 3, -3, 0, 1890, -1, "", 1);// 162
            AddComplexComponent(this, 216, 3, -3, 20, 1890, -1, "", 1);// 163
            AddComplexComponent(this, 222, 3, -3, 40, 1890, -1, "", 1);// 164
            AddComplexComponent(this, 219, 3, -3, 44, 1890, -1, "", 1);// 165
            AddComplexComponent(this, 197, 2, -3, 0, 1890, -1, "", 1);// 166
            AddComplexComponent(this, 197, 2, -3, 20, 1890, -1, "", 1);// 167
            AddComplexComponent(this, 1277, 2, -3, 40, 1890, -1, "", 1);// 168
            AddComplexComponent(this, 197, 1, -3, 0, 1890, -1, "", 1);// 169
            AddComplexComponent(this, 197, 1, -3, 20, 1890, -1, "", 1);// 170
            AddComplexComponent(this, 1276, 1, -3, 40, 1890, -1, "", 1);// 171
            AddComplexComponent(this, 4990, 0, -3, 20, 1266, -1, "", 1);// 173
            AddComplexComponent(this, 4990, -1, -3, 20, 1266, -1, "", 1);// 175
            AddComplexComponent(this, 210, 4, -4, 0, 1890, -1, "", 1);// 177
            AddComplexComponent(this, 204, 3, -4, 0, 1890, -1, "", 1);// 179
            AddComplexComponent(this, 216, 3, -4, 20, 1890, -1, "", 1);// 180
            AddComplexComponent(this, 197, 2, -4, 0, 1890, -1, "", 1);// 181
            AddComplexComponent(this, 197, 2, -4, 20, 1890, -1, "", 1);// 182
            AddComplexComponent(this, 1278, 2, -4, 40, 1890, -1, "", 1);// 183
            AddComplexComponent(this, 221, 2, -4, 40, 1890, -1, "", 1);// 184
            AddComplexComponent(this, 197, 1, -4, 0, 1890, -1, "", 1);// 185
            AddComplexComponent(this, 197, 1, -4, 20, 1890, -1, "", 1);// 186
            AddComplexComponent(this, 1278, 1, -4, 40, 1890, -1, "", 1);// 187
            AddComplexComponent(this, 197, 0, -4, 0, 1890, -1, "", 1);// 188
            AddComplexComponent(this, 197, 0, -4, 20, 1890, -1, "", 1);// 189
            AddComplexComponent(this, 1277, 0, -4, 40, 1890, -1, "", 1);// 190
            AddComplexComponent(this, 4990, -1, -4, 20, 1266, -1, "", 1);// 192
            AddComplexComponent(this, 210, 4, -5, 0, 1890, -1, "", 1);// 193
            AddComplexComponent(this, 204, 3, -5, 0, 1890, -1, "", 1);// 195
            AddComplexComponent(this, 216, 3, -5, 20, 1890, -1, "", 1);// 196
            AddComplexComponent(this, 222, 2, -5, 40, 1890, -1, "", 1);// 197
            AddComplexComponent(this, 217, 2, -5, 20, 1890, -1, "", 1);// 198
            AddComplexComponent(this, 219, 2, -5, 44, 1890, -1, "", 1);// 199
            AddComplexComponent(this, 197, 1, -5, 0, 1890, -1, "", 1);// 200
            AddComplexComponent(this, 197, 1, -5, 20, 1890, -1, "", 1);// 201
            AddComplexComponent(this, 1279, 1, -5, 40, 1890, -1, "", 1);// 202
            AddComplexComponent(this, 221, 1, -5, 40, 1890, -1, "", 1);// 203
            AddComplexComponent(this, 197, 0, -5, 0, 1890, -1, "", 1);// 204
            AddComplexComponent(this, 197, 0, -5, 20, 1890, -1, "", 1);// 205
            AddComplexComponent(this, 1276, 0, -5, 40, 1890, -1, "", 1);// 206
            AddComplexComponent(this, 197, -1, -5, 0, 1890, -1, "", 1);// 207
            AddComplexComponent(this, 197, -1, -5, 20, 1890, -1, "", 1);// 208
            AddComplexComponent(this, 1276, -1, -5, 40, 1890, -1, "", 1);// 209
            AddComplexComponent(this, 210, 3, -6, 0, 1890, -1, "", 1);// 210
            AddComplexComponent(this, 216, 2, -6, 20, 1890, -1, "", 1);// 211
            AddComplexComponent(this, 204, 2, -6, 0, 1890, -1, "", 1);// 212
            AddComplexComponent(this, 222, 1, -6, 40, 1890, -1, "", 1);// 213
            AddComplexComponent(this, 200, 1, -6, 20, 1890, -1, "", 1);// 214
            AddComplexComponent(this, 200, 1, -6, 0, 1890, -1, "", 1);// 215
            AddComplexComponent(this, 217, 1, -6, 20, 1890, -1, "", 1);// 216
            AddComplexComponent(this, 219, 1, -6, 44, 1890, -1, "", 1);// 217
            AddComplexComponent(this, 216, 0, -6, 20, 1890, -1, "", 1);// 218
            AddComplexComponent(this, 222, 0, -6, 40, 1890, -1, "", 1);// 219
            AddComplexComponent(this, 217, 0, -6, 20, 1890, -1, "", 1);// 220
            AddComplexComponent(this, 197, -1, -6, 0, 1890, -1, "", 1);// 221
            AddComplexComponent(this, 197, -1, -6, 20, 1890, -1, "", 1);// 222
            AddComplexComponent(this, 1279, -1, -6, 40, 1890, -1, "", 1);// 223
            AddComplexComponent(this, 221, -1, -6, 40, 1890, -1, "", 1);// 224
            AddComplexComponent(this, 222, -1, -7, 40, 1890, -1, "", 1);// 225
            AddComplexComponent(this, 200, -1, -7, 20, 1890, -1, "", 1);// 226
            AddComplexComponent(this, 219, -1, -7, 44, 1890, -1, "", 1);// 227
            AddComplexComponent(this, 211, -2, 6, 0, 1883, -1, "", 1);// 228
            AddComplexComponent(this, 211, -3, 6, 0, 1882, -1, "", 1);// 230
            AddComplexComponent(this, 211, -4, 6, 0, 1882, -1, "", 1);// 232
            AddComplexComponent(this, 211, -5, 6, 0, 1882, -1, "", 1);// 234
            AddComplexComponent(this, 211, -6, 6, 0, 1882, -1, "", 1);// 236
            AddComplexComponent(this, 211, -7, 6, 0, 1882, -1, "", 1);// 238
            AddComplexComponent(this, 197, -2, 5, 0, 1883, -1, "", 1);// 240
            AddComplexComponent(this, 197, -2, 5, 20, 1883, -1, "", 1);// 241
            AddComplexComponent(this, 1278, -2, 5, 40, 1883, -1, "", 1);// 242
            AddComplexComponent(this, 222, -2, 5, 40, 1883, -1, "", 1);// 243
            AddComplexComponent(this, 197, -3, 5, 0, 1882, -1, "", 1);// 244
            AddComplexComponent(this, 197, -3, 5, 20, 1882, -1, "", 1);// 245
            AddComplexComponent(this, 1277, -3, 5, 40, 1882, -1, "", 1);// 246
            AddComplexComponent(this, 222, -3, 5, 40, 1882, -1, "", 1);// 247
            AddComplexComponent(this, 197, -4, 5, 0, 1882, -1, "", 1);// 248
            AddComplexComponent(this, 197, -4, 5, 20, 1882, -1, "", 1);// 249
            AddComplexComponent(this, 1276, -4, 5, 40, 1882, -1, "", 1);// 250
            AddComplexComponent(this, 222, -4, 5, 40, 1882, -1, "", 1);// 251
            AddComplexComponent(this, 204, -5, 5, 0, 1882, -1, "", 1);// 252
            AddComplexComponent(this, 215, -5, 5, 20, 1882, -1, "", 1);// 253
            AddComplexComponent(this, 221, -5, 5, 40, 1882, -1, "", 1);// 254
            AddComplexComponent(this, 219, -5, 5, 44, 1882, -1, "", 1);// 255
            AddComplexComponent(this, 204, -6, 5, 0, 1882, -1, "", 1);// 256
            AddComplexComponent(this, 215, -6, 5, 20, 1882, -1, "", 1);// 257
            AddComplexComponent(this, 204, -7, 5, 0, 1882, -1, "", 1);// 258
            AddComplexComponent(this, 215, -7, 5, 20, 1882, -1, "", 1);// 259
            AddComplexComponent(this, 211, -8, 5, 0, 1882, -1, "", 1);// 260
            AddComplexComponent(this, 197, -2, 4, 0, 1882, -1, "", 1);// 261
            AddComplexComponent(this, 197, -2, 4, 20, 1882, -1, "", 1);// 262
            AddComplexComponent(this, 1277, -2, 4, 40, 1884, -1, "", 1);// 263
            AddComplexComponent(this, 197, -3, 4, 0, 1882, -1, "", 1);// 264
            AddComplexComponent(this, 197, -3, 4, 20, 1882, -1, "", 1);// 265
            AddComplexComponent(this, 1278, -3, 4, 40, 1883, -1, "", 1);// 266
            AddComplexComponent(this, 197, -4, 4, 0, 1882, -1, "", 1);// 267
            AddComplexComponent(this, 197, -4, 4, 20, 1882, -1, "", 1);// 268
            AddComplexComponent(this, 1279, -4, 4, 40, 1882, -1, "", 1);// 269
            AddComplexComponent(this, 197, -5, 4, 0, 1883, -1, "", 1);// 270
            AddComplexComponent(this, 197, -5, 4, 20, 1883, -1, "", 1);// 271
            AddComplexComponent(this, 1276, -5, 4, 40, 1882, -1, "", 1);// 272
            AddComplexComponent(this, 197, -6, 4, 0, 1882, -1, "", 1);// 273
            AddComplexComponent(this, 197, -6, 4, 20, 1882, -1, "", 1);// 274
            AddComplexComponent(this, 1277, -6, 4, 40, 1882, -1, "", 1);// 275
            AddComplexComponent(this, 222, -6, 4, 40, 1882, -1, "", 1);// 276
            AddComplexComponent(this, 215, -7, 4, 20, 1882, -1, "", 1);// 277
            AddComplexComponent(this, 221, -7, 4, 40, 1882, -1, "", 1);// 278
            AddComplexComponent(this, 218, -7, 4, 20, 1882, -1, "", 1);// 279
            AddComplexComponent(this, 219, -7, 4, 44, 1882, -1, "", 1);// 280
            AddComplexComponent(this, 215, -8, 4, 20, 1882, -1, "", 1);// 281
            AddComplexComponent(this, 204, -8, 4, 0, 1882, -1, "", 1);// 282
            AddComplexComponent(this, 197, -5, 3, 0, 1883, -1, "", 1);// 283
            AddComplexComponent(this, 197, -5, 3, 20, 1883, -1, "", 1);// 284
            AddComplexComponent(this, 1279, -5, 3, 40, 1882, -1, "", 1);// 285
            AddComplexComponent(this, 197, -6, 3, 0, 1882, -1, "", 1);// 286
            AddComplexComponent(this, 197, -6, 3, 20, 1882, -1, "", 1);// 287
            AddComplexComponent(this, 1278, -6, 3, 40, 1882, -1, "", 1);// 288
            AddComplexComponent(this, 197, -7, 3, 0, 1882, -1, "", 1);// 289
            AddComplexComponent(this, 197, -7, 3, 20, 1882, -1, "", 1);// 290
            AddComplexComponent(this, 1278, -7, 3, 40, 1882, -1, "", 1);// 291
            AddComplexComponent(this, 222, -7, 3, 40, 1882, -1, "", 1);// 292
            AddComplexComponent(this, 221, -8, 3, 40, 1882, -1, "", 1);// 293
            AddComplexComponent(this, 201, -8, 3, 20, 1882, -1, "", 1);// 294
            AddComplexComponent(this, 218, -8, 3, 20, 1882, -1, "", 1);// 295
            AddComplexComponent(this, 219, -8, 3, 44, 1882, -1, "", 1);// 296
            AddComplexComponent(this, 4990, -2, 2, 20, 1266, -1, "", 1);// 298
            AddComplexComponent(this, 4990, -3, 2, 20, 1266, -1, "", 1);// 300
            AddComplexComponent(this, 4990, -4, 2, 20, 1266, -1, "", 1);// 302
            AddComplexComponent(this, 4990, -5, 2, 20, 1266, -1, "", 1);// 304
            AddComplexComponent(this, 197, -6, 2, 0, 1882, -1, "", 1);// 305
            AddComplexComponent(this, 197, -6, 2, 20, 1882, -1, "", 1);// 306
            AddComplexComponent(this, 1276, -6, 2, 40, 1882, -1, "", 1);// 307
            AddComplexComponent(this, 197, -7, 2, 0, 1883, -1, "", 1);// 308
            AddComplexComponent(this, 197, -7, 2, 20, 1882, -1, "", 1);// 309
            AddComplexComponent(this, 1277, -7, 2, 40, 1882, -1, "", 1);// 310
            AddComplexComponent(this, 221, -8, 2, 40, 1882, -1, "", 1);// 311
            AddComplexComponent(this, 218, -8, 2, 20, 1882, -1, "", 1);// 312
            AddComplexComponent(this, 204, -9, 2, 0, 1882, -1, "", 1);// 313
            AddComplexComponent(this, 4990, -2, 1, 20, 1266, -1, "", 1);// 315
            AddComplexComponent(this, 4990, -3, 1, 20, 1266, -1, "", 1);// 317
            AddComplexComponent(this, 4990, -4, 1, 20, 1266, -1, "", 1);// 319
            AddComplexComponent(this, 4990, -5, 1, 20, 1266, -1, "", 1);// 321
            AddComplexComponent(this, 4990, -6, 1, 20, 1266, -1, "", 1);// 323
            AddComplexComponent(this, 197, -7, 1, 0, 1882, -1, "", 1);// 324
            AddComplexComponent(this, 197, -7, 1, 20, 1883, -1, "", 1);// 325
            AddComplexComponent(this, 1279, -7, 1, 40, 1883, -1, "", 1);// 326
            AddComplexComponent(this, 197, -8, 1, 0, 1882, -1, "", 1);// 327
            AddComplexComponent(this, 197, -8, 1, 20, 1882, -1, "", 1);// 328
            AddComplexComponent(this, 1276, -8, 1, 40, 1882, -1, "", 1);// 329
            AddComplexComponent(this, 222, -8, 1, 40, 1882, -1, "", 1);// 330
            AddComplexComponent(this, 221, -9, 1, 40, 1882, -1, "", 1);// 331
            AddComplexComponent(this, 201, -9, 1, 20, 1882, -1, "", 1);// 332
            AddComplexComponent(this, 219, -9, 1, 44, 1882, -1, "", 1);// 333
            AddComplexComponent(this, 4990, -2, 0, 20, 1266, -1, "", 1);// 335
            AddComplexComponent(this, 4990, -3, 0, 20, 1266, -1, "", 1);// 337
            AddComplexComponent(this, 4990, -4, 0, 20, 1266, -1, "", 1);// 339
            AddComplexComponent(this, 4990, -5, 0, 20, 1266, -1, "", 1);// 341
            AddComplexComponent(this, 4990, -6, 0, 20, 1266, -1, "", 1);// 343
            AddComplexComponent(this, 197, -7, 0, 0, 1884, -1, "", 1);// 344
            AddComplexComponent(this, 197, -7, 0, 20, 1884, -1, "", 1);// 345
            AddComplexComponent(this, 1278, -7, 0, 40, 1884, -1, "", 1);// 346
            AddComplexComponent(this, 197, -8, 0, 0, 1883, -1, "", 1);// 347
            AddComplexComponent(this, 197, -8, 0, 20, 1883, -1, "", 1);// 348
            AddComplexComponent(this, 1277, -8, 0, 40, 1883, -1, "", 1);// 349
            AddComplexComponent(this, 221, -9, 0, 40, 1882, -1, "", 1);// 350
            AddComplexComponent(this, 4990, -2, -1, 20, 1266, -1, "", 1);// 352
            AddComplexComponent(this, 4990, -3, -1, 20, 1266, -1, "", 1);// 354
            AddComplexComponent(this, 4990, -4, -1, 20, 1266, -1, "", 1);// 356
            AddComplexComponent(this, 4990, -5, -1, 20, 1266, -1, "", 1);// 358
            AddComplexComponent(this, 4990, -6, -1, 20, 1266, -1, "", 1);// 360
            AddComplexComponent(this, 197, -7, -1, 0, 1885, -1, "", 1);// 361
            AddComplexComponent(this, 197, -7, -1, 20, 1885, -1, "", 1);// 362
            AddComplexComponent(this, 1277, -7, -1, 40, 1885, -1, "", 1);// 363
            AddComplexComponent(this, 197, -8, -1, 0, 1884, -1, "", 1);// 364
            AddComplexComponent(this, 197, -8, -1, 20, 1884, -1, "", 1);// 365
            AddComplexComponent(this, 1278, -8, -1, 40, 1884, -1, "", 1);// 366
            AddComplexComponent(this, 221, -9, -1, 40, 1883, -1, "", 1);// 367
            AddComplexComponent(this, 4990, -2, -2, 20, 1266, -1, "", 1);// 369
            AddComplexComponent(this, 4990, -3, -2, 20, 1266, -1, "", 1);// 371
            AddComplexComponent(this, 4990, -4, -2, 20, 1266, -1, "", 1);// 373
            AddComplexComponent(this, 4990, -5, -2, 20, 1266, -1, "", 1);// 375
            AddComplexComponent(this, 4990, -6, -2, 20, 1266, -1, "", 1);// 377
            AddComplexComponent(this, 197, -7, -2, 0, 1886, -1, "", 1);// 378
            AddComplexComponent(this, 197, -7, -2, 20, 1886, -1, "", 1);// 379
            AddComplexComponent(this, 1276, -7, -2, 40, 1886, -1, "", 1);// 380
            AddComplexComponent(this, 197, -8, -2, 0, 1885, -1, "", 1);// 381
            AddComplexComponent(this, 197, -8, -2, 20, 1885, -1, "", 1);// 382
            AddComplexComponent(this, 1279, -8, -2, 40, 1885, -1, "", 1);// 383
            AddComplexComponent(this, 221, -9, -2, 40, 1884, -1, "", 1);// 384
            AddComplexComponent(this, 4990, -2, -3, 20, 1266, -1, "", 1);// 386
            AddComplexComponent(this, 4990, -3, -3, 20, 1266, -1, "", 1);// 388
            AddComplexComponent(this, 4990, -4, -3, 20, 1266, -1, "", 1);// 390
            AddComplexComponent(this, 4990, -5, -3, 20, 1266, -1, "", 1);// 392
            AddComplexComponent(this, 197, -6, -3, 0, 1887, -1, "", 1);// 393
            AddComplexComponent(this, 197, -6, -3, 20, 1887, -1, "", 1);// 394
            AddComplexComponent(this, 1276, -6, -3, 40, 1887, -1, "", 1);// 395
            AddComplexComponent(this, 197, -7, -3, 0, 1886, -1, "", 1);// 396
            AddComplexComponent(this, 197, -7, -3, 20, 1886, -1, "", 1);// 397
            AddComplexComponent(this, 1279, -7, -3, 40, 1886, -1, "", 1);// 398
            AddComplexComponent(this, 220, -8, -3, 40, 1885, -1, "", 1);// 399
            AddComplexComponent(this, 223, -9, -3, 40, 1885, -1, "", 1);// 400
            AddComplexComponent(this, 219, -9, -3, 44, 1885, -1, "", 1);// 401
            AddComplexComponent(this, 4990, -2, -4, 20, 1266, -1, "", 1);// 403
            AddComplexComponent(this, 4990, -3, -4, 20, 1266, -1, "", 1);// 405
            AddComplexComponent(this, 4990, -4, -4, 20, 1266, -1, "", 1);// 407
            AddComplexComponent(this, 197, -5, -4, 0, 1889, -1, "", 1);// 408
            AddComplexComponent(this, 197, -5, -4, 20, 1889, -1, "", 1);// 409
            AddComplexComponent(this, 1277, -5, -4, 40, 1888, -1, "", 1);// 410
            AddComplexComponent(this, 197, -6, -4, 0, 1887, -1, "", 1);// 411
            AddComplexComponent(this, 197, -6, -4, 20, 1887, -1, "", 1);// 412
            AddComplexComponent(this, 1277, -6, -4, 40, 1887, -1, "", 1);// 413
            AddComplexComponent(this, 197, -7, -4, 0, 1887, -1, "", 1);// 414
            AddComplexComponent(this, 197, -7, -4, 20, 1887, -1, "", 1);// 415
            AddComplexComponent(this, 1278, -7, -4, 40, 1887, -1, "", 1);// 416
            AddComplexComponent(this, 221, -8, -4, 40, 1886, -1, "", 1);// 417
            AddComplexComponent(this, 197, -2, -5, 0, 1890, -1, "", 1);// 418
            AddComplexComponent(this, 197, -2, -5, 20, 1890, -1, "", 1);// 419
            AddComplexComponent(this, 1277, -2, -5, 40, 1890, -1, "", 1);// 420
            AddComplexComponent(this, 197, -3, -5, 0, 1890, -1, "", 1);// 421
            AddComplexComponent(this, 197, -3, -5, 20, 1890, -1, "", 1);// 422
            AddComplexComponent(this, 1278, -3, -5, 40, 1890, -1, "", 1);// 423
            AddComplexComponent(this, 197, -4, -5, 0, 1890, -1, "", 1);// 424
            AddComplexComponent(this, 197, -4, -5, 20, 1890, -1, "", 1);// 425
            AddComplexComponent(this, 1279, -4, -5, 40, 1890, -1, "", 1);// 426
            AddComplexComponent(this, 197, -5, -5, 0, 1888, -1, "", 1);// 427
            AddComplexComponent(this, 197, -5, -5, 20, 1888, -1, "", 1);// 428
            AddComplexComponent(this, 1278, -5, -5, 40, 1889, -1, "", 1);// 429
            AddComplexComponent(this, 197, -6, -5, 0, 1888, -1, "", 1);// 430
            AddComplexComponent(this, 197, -6, -5, 20, 1888, -1, "", 1);// 431
            AddComplexComponent(this, 1279, -6, -5, 40, 1888, -1, "", 1);// 432
            AddComplexComponent(this, 220, -7, -5, 40, 1887, -1, "", 1);// 433
            AddComplexComponent(this, 223, -8, -5, 40, 1887, -1, "", 1);// 434
            AddComplexComponent(this, 219, -8, -5, 44, 1887, -1, "", 1);// 435
            AddComplexComponent(this, 197, -2, -6, 0, 1890, -1, "", 1);// 436
            AddComplexComponent(this, 197, -2, -6, 20, 1890, -1, "", 1);// 437
            AddComplexComponent(this, 1278, -2, -6, 40, 1890, -1, "", 1);// 438
            AddComplexComponent(this, 197, -3, -6, 0, 1890, -1, "", 1);// 439
            AddComplexComponent(this, 197, -3, -6, 20, 1890, -1, "", 1);// 440
            AddComplexComponent(this, 1277, -3, -6, 40, 1890, -1, "", 1);// 441
            AddComplexComponent(this, 197, -4, -6, 0, 1890, -1, "", 1);// 442
            AddComplexComponent(this, 197, -4, -6, 20, 1890, -1, "", 1);// 443
            AddComplexComponent(this, 1276, -4, -6, 40, 1890, -1, "", 1);// 444
            AddComplexComponent(this, 220, -5, -6, 40, 1889, -1, "", 1);// 445
            AddComplexComponent(this, 222, -6, -6, 40, 1888, -1, "", 1);// 446
            AddComplexComponent(this, 223, -7, -6, 40, 1888, -1, "", 1);// 447
            AddComplexComponent(this, 219, -7, -6, 44, 1888, -1, "", 1);// 448
            AddComplexComponent(this, 222, -2, -7, 40, 1890, -1, "", 1);// 449
            AddComplexComponent(this, 222, -3, -7, 40, 1890, -1, "", 1);// 450
            AddComplexComponent(this, 222, -4, -7, 40, 1890, -1, "", 1);// 451
            AddComplexComponent(this, 223, -5, -7, 40, 1890, -1, "", 1);// 452
            AddComplexComponent(this, 219, -5, -7, 44, 1890, -1, "", 1);// 453
        }

        public IversRoundingAddon(Serial serial)
            : base(serial)
        {
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (!string.IsNullOrEmpty(name))
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType)lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
