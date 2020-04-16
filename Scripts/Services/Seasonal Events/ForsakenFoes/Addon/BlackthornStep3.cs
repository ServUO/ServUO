namespace Server.Items
{
    public class BlackthornStep3 : BlackthornBaseAddon
    {
        public static BlackthornStep3 InstanceTram { get; set; }
        public static BlackthornStep3 InstanceFel { get; set; }

        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
              {6093, 11, -4, 0}, {6093, 12, 0, 0}, {6093, 10, 1, 0}// 1	2	3	
			, {6093, 10, -2, 0}, {6670, 10, -9, 0}, {6093, 12, -7, 0}// 4	55	67	
			, {6093, 10, -7, 0}, {6093, 9, 3, 0}, {6093, 8, 3, 0}// 68	88	89	
			, {6093, 9, 5, 0}, {6093, 7, 5, 0}, {6093, 7, 0, 0}// 90	91	92	
			, {6093, 5, 1, 0}, {6093, 8, -2, 0}, {6093, 5, -2, 0}// 93	94	95	
			, {6093, 9, -5, 0}, {6093, 6, -5, 0}, {6093, 4, -5, 0}// 96	97	98	
			, {6093, 2, -3, 0}, {6093, 1, -5, 0}, {6093, -3, -4, 0}// 99	100	101	
			, {6093, -2, -2, 0}, {6093, -4, -2, 0}, {6093, -6, -5, 0}// 102	103	104	
			, {6093, 3, 0, 0}, {6093, 4, 0, 0}, {6093, 4, 1, 0}// 105	106	107	
			, {6093, 2, 1, 0}, {6093, 1, 0, 0}, {6093, -1, 0, 0}// 108	109	110	
			, {6093, -3, 0, 0}, {6093, -5, 1, 0}, {6093, 8, -7, 0}// 111	112	281	
			, {6093, 7, -8, 0}, {6093, 5, -8, 0}, {6093, 3, -8, 0}// 282	283	284	
			, {6093, 0, -6, 0}, {6093, -1, -8, 0}, {6093, -4, -6, 0}// 285	286	287	
			, {6093, -5, -8, 0}, {6093, -10, 5, 0}, {6093, -11, 4, 0}// 288	373	374	
			, {6093, -10, 7, 0}, {6093, -9, 6, 0}, {6093, -12, 7, 0}// 375	376	377	
			, {6093, -7, -2, 0}, {6093, -7, -1, 0}, {6093, -9, -3, 0}// 378	379	380	
			, {6093, -7, -4, 0}, {6093, -7, 1, 0}, {6093, -10, 0, 0}// 381	382	383	
			, {6093, -9, 1, 0}, {6093, -12, 0, 0}, {6093, -12, -4, 0}// 384	385	386	
			, {6093, -7, -8, 0}, {6093, -9, -8, 0}, {6093, -11, -6, 0}// 489	490	491	
			, {6093, -11, -8, 0}// 492	
		};

        [Constructable]
        public BlackthornStep3()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 6077, 12, 8, 0, 2729, -1, "", 1);// 5
            AddComplexComponent(this, 6077, 12, 7, 0, 2729, -1, "", 1);// 6
            AddComplexComponent(this, 6077, 12, 6, 0, 2729, -1, "", 1);// 7
            AddComplexComponent(this, 6077, 12, 5, 0, 2729, -1, "", 1);// 8
            AddComplexComponent(this, 6077, 12, 4, 0, 2729, -1, "", 1);// 9
            AddComplexComponent(this, 6077, 12, 3, 0, 2729, -1, "", 1);// 10
            AddComplexComponent(this, 6077, 11, 8, 0, 2729, -1, "", 1);// 11
            AddComplexComponent(this, 6077, 11, 7, 0, 2729, -1, "", 1);// 12
            AddComplexComponent(this, 6077, 11, 6, 0, 2729, -1, "", 1);// 13
            AddComplexComponent(this, 6077, 11, 5, 0, 2729, -1, "", 1);// 14
            AddComplexComponent(this, 6077, 11, 4, 0, 2729, -1, "", 1);// 15
            AddComplexComponent(this, 6077, 11, 3, 0, 2729, -1, "", 1);// 16
            AddComplexComponent(this, 6077, 10, 8, 0, 2729, -1, "", 1);// 17
            AddComplexComponent(this, 6077, 10, 7, 0, 2729, -1, "", 1);// 18
            AddComplexComponent(this, 6077, 10, 6, 0, 2729, -1, "", 1);// 19
            AddComplexComponent(this, 6077, 10, 5, 0, 2729, -1, "", 1);// 20
            AddComplexComponent(this, 6077, 10, 4, 0, 2729, -1, "", 1);// 21
            AddComplexComponent(this, 6077, 10, 3, 0, 2729, -1, "", 1);// 22
            AddComplexComponent(this, 6077, 13, 2, 0, 2729, -1, "", 1);// 23
            AddComplexComponent(this, 6077, 13, 1, 0, 2729, -1, "", 1);// 24
            AddComplexComponent(this, 6077, 13, 0, 0, 2729, -1, "", 1);// 25
            AddComplexComponent(this, 6077, 13, -1, 0, 2729, -1, "", 1);// 26
            AddComplexComponent(this, 6077, 12, 2, 0, 2729, -1, "", 1);// 27
            AddComplexComponent(this, 6077, 12, 1, 0, 2729, -1, "", 1);// 28
            AddComplexComponent(this, 6077, 12, 0, 0, 2729, -1, "", 1);// 29
            AddComplexComponent(this, 6077, 12, -1, 0, 2729, -1, "", 1);// 30
            AddComplexComponent(this, 6077, 11, 2, 0, 2729, -1, "", 1);// 31
            AddComplexComponent(this, 6077, 11, 1, 0, 2729, -1, "", 1);// 32
            AddComplexComponent(this, 6077, 11, 0, 0, 2729, -1, "", 1);// 33
            AddComplexComponent(this, 6077, 11, -1, 0, 2729, -1, "", 1);// 34
            AddComplexComponent(this, 6077, 10, 2, 0, 2729, -1, "", 1);// 35
            AddComplexComponent(this, 6077, 10, 1, 0, 2729, -1, "", 1);// 36
            AddComplexComponent(this, 6077, 10, 0, 0, 2729, -1, "", 1);// 37
            AddComplexComponent(this, 6077, 10, -1, 0, 2729, -1, "", 1);// 38
            AddComplexComponent(this, 6077, 13, -2, 0, 2729, -1, "", 1);// 39
            AddComplexComponent(this, 6077, 13, -3, 0, 2729, -1, "", 1);// 40
            AddComplexComponent(this, 6077, 13, -4, 0, 2729, -1, "", 1);// 41
            AddComplexComponent(this, 6077, 13, -5, 0, 2729, -1, "", 1);// 42
            AddComplexComponent(this, 6077, 12, -2, 0, 2729, -1, "", 1);// 43
            AddComplexComponent(this, 6077, 12, -3, 0, 2729, -1, "", 1);// 44
            AddComplexComponent(this, 6077, 12, -4, 0, 2729, -1, "", 1);// 45
            AddComplexComponent(this, 6077, 12, -5, 0, 2729, -1, "", 1);// 46
            AddComplexComponent(this, 6077, 11, -2, 0, 2729, -1, "", 1);// 47
            AddComplexComponent(this, 6077, 11, -3, 0, 2729, -1, "", 1);// 48
            AddComplexComponent(this, 6077, 11, -4, 0, 2729, -1, "", 1);// 49
            AddComplexComponent(this, 6077, 11, -5, 0, 2729, -1, "", 1);// 50
            AddComplexComponent(this, 6077, 10, -2, 0, 2729, -1, "", 1);// 51
            AddComplexComponent(this, 6077, 10, -3, 0, 2729, -1, "", 1);// 52
            AddComplexComponent(this, 6077, 10, -4, 0, 2729, -1, "", 1);// 53
            AddComplexComponent(this, 6077, 10, -5, 0, 2729, -1, "", 1);// 54
            AddComplexComponent(this, 7073, 12, -7, 0, 2729, -1, "", 1);// 56
            AddComplexComponent(this, 7072, 11, -7, 0, 2729, -1, "", 1);// 57
            AddComplexComponent(this, 7074, 10, -7, 0, 2729, -1, "", 1);// 58
            AddComplexComponent(this, 7076, 10, -8, 0, 2729, -1, "", 1);// 59
            AddComplexComponent(this, 7077, 10, -9, 0, 2729, -1, "", 1);// 60
            AddComplexComponent(this, 7079, 11, -8, 0, 2729, -1, "", 1);// 61
            AddComplexComponent(this, 7080, 12, -8, 0, 2729, -1, "", 1);// 62
            AddComplexComponent(this, 7080, 11, -9, 0, 2729, -1, "", 1);// 63
            AddComplexComponent(this, 7080, 12, -9, 0, 2729, -1, "", 1);// 64
            AddComplexComponent(this, 17778, 11, -9, 0, 1152, -1, "", 1);// 65
            AddComplexComponent(this, 17778, 12, -9, 0, 1152, -1, "", 1);// 66
            AddComplexComponent(this, 6077, 13, -8, 0, 2729, -1, "", 1);// 69
            AddComplexComponent(this, 6077, 13, -9, 0, 2729, -1, "", 1);// 70
            AddComplexComponent(this, 6077, 12, -8, 0, 2729, -1, "", 1);// 71
            AddComplexComponent(this, 6077, 12, -9, 0, 2729, -1, "", 1);// 72
            AddComplexComponent(this, 6077, 11, -8, 0, 2729, -1, "", 1);// 73
            AddComplexComponent(this, 6077, 11, -9, 0, 2729, -1, "", 1);// 74
            AddComplexComponent(this, 6077, 10, -8, 0, 2729, -1, "", 1);// 75
            AddComplexComponent(this, 6077, 10, -9, 0, 2729, -1, "", 1);// 76
            AddComplexComponent(this, 6077, 13, -6, 0, 2729, -1, "", 1);// 77
            AddComplexComponent(this, 6077, 13, -7, 0, 2729, -1, "", 1);// 78
            AddComplexComponent(this, 6077, 12, -6, 0, 2729, -1, "", 1);// 79
            AddComplexComponent(this, 6077, 12, -7, 0, 2729, -1, "", 1);// 80
            AddComplexComponent(this, 6077, 11, -6, 0, 2729, -1, "", 1);// 81
            AddComplexComponent(this, 6077, 11, -7, 0, 2729, -1, "", 1);// 82
            AddComplexComponent(this, 6077, 10, -6, 0, 2729, -1, "", 1);// 83
            AddComplexComponent(this, 6077, 10, -7, 0, 2729, -1, "", 1);// 84
            AddComplexComponent(this, 7820, 4, -1, 0, 2729, -1, "", 1);// 85
            AddComplexComponent(this, 7822, 3, -2, 0, 2729, -1, "", 1);// 86
            AddComplexComponent(this, 7825, -2, -3, 0, 1152, -1, "", 1);// 87
            AddComplexComponent(this, 22300, 9, 0, 0, 1152, -1, "", 1);// 113
            AddComplexComponent(this, 22300, -6, -3, 0, 1152, -1, "", 1);// 114
            AddComplexComponent(this, 22300, 1, -1, 0, 1152, -1, "", 1);// 115
            AddComplexComponent(this, 7398, 8, 5, 0, 2729, -1, "", 1);// 116
            AddComplexComponent(this, 6077, 6, 3, 0, 2729, -1, "", 1);// 117
            AddComplexComponent(this, 6077, 9, 8, 0, 2729, -1, "", 1);// 118
            AddComplexComponent(this, 6077, 9, 7, 0, 2729, -1, "", 1);// 119
            AddComplexComponent(this, 6077, 9, 6, 0, 2729, -1, "", 1);// 120
            AddComplexComponent(this, 6077, 9, 5, 0, 2729, -1, "", 1);// 121
            AddComplexComponent(this, 6077, 9, 4, 0, 2729, -1, "", 1);// 122
            AddComplexComponent(this, 6077, 9, 3, 0, 2729, -1, "", 1);// 123
            AddComplexComponent(this, 6077, 8, 8, 0, 2729, -1, "", 1);// 124
            AddComplexComponent(this, 6077, 8, 7, 0, 2729, -1, "", 1);// 125
            AddComplexComponent(this, 6077, 8, 6, 0, 2729, -1, "", 1);// 126
            AddComplexComponent(this, 6077, 8, 5, 0, 2729, -1, "", 1);// 127
            AddComplexComponent(this, 6077, 8, 4, 0, 2729, -1, "", 1);// 128
            AddComplexComponent(this, 6077, 8, 3, 0, 2729, -1, "", 1);// 129
            AddComplexComponent(this, 6077, 7, 8, 0, 2729, -1, "", 1);// 130
            AddComplexComponent(this, 6077, 7, 7, 0, 2729, -1, "", 1);// 131
            AddComplexComponent(this, 6077, 7, 6, 0, 2729, -1, "", 1);// 132
            AddComplexComponent(this, 6077, 7, 5, 0, 2729, -1, "", 1);// 133
            AddComplexComponent(this, 6077, 7, 4, 0, 2729, -1, "", 1);// 134
            AddComplexComponent(this, 6077, 7, 3, 0, 2729, -1, "", 1);// 135
            AddComplexComponent(this, 6077, 9, 2, 0, 2729, -1, "", 1);// 136
            AddComplexComponent(this, 6077, 9, 1, 0, 2729, -1, "", 1);// 137
            AddComplexComponent(this, 6077, 9, 0, 0, 2729, -1, "", 1);// 138
            AddComplexComponent(this, 6077, 9, -1, 0, 2729, -1, "", 1);// 139
            AddComplexComponent(this, 6077, 8, 2, 0, 2729, -1, "", 1);// 140
            AddComplexComponent(this, 6077, 8, 1, 0, 2729, -1, "", 1);// 141
            AddComplexComponent(this, 6077, 8, 0, 0, 2729, -1, "", 1);// 142
            AddComplexComponent(this, 6077, 8, -1, 0, 2729, -1, "", 1);// 143
            AddComplexComponent(this, 6077, 7, 2, 0, 2729, -1, "", 1);// 144
            AddComplexComponent(this, 6077, 7, 1, 0, 2729, -1, "", 1);// 145
            AddComplexComponent(this, 6077, 7, 0, 0, 2729, -1, "", 1);// 146
            AddComplexComponent(this, 6077, 7, -1, 0, 2729, -1, "", 1);// 147
            AddComplexComponent(this, 6077, 6, 2, 0, 2729, -1, "", 1);// 148
            AddComplexComponent(this, 6077, 6, 1, 0, 2729, -1, "", 1);// 149
            AddComplexComponent(this, 6077, 6, 0, 0, 2729, -1, "", 1);// 150
            AddComplexComponent(this, 6077, 6, -1, 0, 2729, -1, "", 1);// 151
            AddComplexComponent(this, 6077, 5, 2, 0, 2729, -1, "", 1);// 152
            AddComplexComponent(this, 6077, 5, 1, 0, 2729, -1, "", 1);// 153
            AddComplexComponent(this, 6077, 5, 0, 0, 2729, -1, "", 1);// 154
            AddComplexComponent(this, 6077, 5, -1, 0, 2729, -1, "", 1);// 155
            AddComplexComponent(this, 6077, 4, 2, 0, 2729, -1, "", 1);// 156
            AddComplexComponent(this, 6077, 4, 1, 0, 2729, -1, "", 1);// 157
            AddComplexComponent(this, 6077, 4, 0, 0, 2729, -1, "", 1);// 158
            AddComplexComponent(this, 6077, 4, -1, 0, 2729, -1, "", 1);// 159
            AddComplexComponent(this, 6077, 3, 2, 0, 2729, -1, "", 1);// 160
            AddComplexComponent(this, 6077, 3, 1, 0, 2729, -1, "", 1);// 161
            AddComplexComponent(this, 6077, 3, 0, 0, 2729, -1, "", 1);// 162
            AddComplexComponent(this, 6077, 3, -1, 0, 2729, -1, "", 1);// 163
            AddComplexComponent(this, 6077, 2, 2, 0, 2729, -1, "", 1);// 164
            AddComplexComponent(this, 6077, 2, 1, 0, 2729, -1, "", 1);// 165
            AddComplexComponent(this, 6077, 2, 0, 0, 2729, -1, "", 1);// 166
            AddComplexComponent(this, 6077, 2, -1, 0, 2729, -1, "", 1);// 167
            AddComplexComponent(this, 6077, 1, 2, 0, 2729, -1, "", 1);// 168
            AddComplexComponent(this, 6077, 1, 1, 0, 2729, -1, "", 1);// 169
            AddComplexComponent(this, 6077, 1, 0, 0, 2729, -1, "", 1);// 170
            AddComplexComponent(this, 6077, 1, -1, 0, 2729, -1, "", 1);// 171
            AddComplexComponent(this, 6077, 0, 2, 0, 2729, -1, "", 1);// 172
            AddComplexComponent(this, 6077, 0, 1, 0, 2729, -1, "", 1);// 173
            AddComplexComponent(this, 6077, 0, 0, 0, 2729, -1, "", 1);// 174
            AddComplexComponent(this, 6077, 0, -1, 0, 2729, -1, "", 1);// 175
            AddComplexComponent(this, 6077, 0, -2, 0, 2729, -1, "", 1);// 176
            AddComplexComponent(this, 6077, -1, 2, 0, 2729, -1, "", 1);// 177
            AddComplexComponent(this, 6077, -1, 1, 0, 2729, -1, "", 1);// 178
            AddComplexComponent(this, 6077, -1, 0, 0, 2729, -1, "", 1);// 179
            AddComplexComponent(this, 6077, -1, -1, 0, 2729, -1, "", 1);// 180
            AddComplexComponent(this, 6077, -1, -2, 0, 2729, -1, "", 1);// 181
            AddComplexComponent(this, 6077, -2, 2, 0, 2729, -1, "", 1);// 182
            AddComplexComponent(this, 6077, -2, 1, 0, 2729, -1, "", 1);// 183
            AddComplexComponent(this, 6077, -2, 0, 0, 2729, -1, "", 1);// 184
            AddComplexComponent(this, 6077, -2, -1, 0, 2729, -1, "", 1);// 185
            AddComplexComponent(this, 6077, -2, -2, 0, 2729, -1, "", 1);// 186
            AddComplexComponent(this, 6077, -3, 2, 0, 2729, -1, "", 1);// 187
            AddComplexComponent(this, 6077, -3, 1, 0, 2729, -1, "", 1);// 188
            AddComplexComponent(this, 6077, -3, 0, 0, 2729, -1, "", 1);// 189
            AddComplexComponent(this, 6077, -3, -1, 0, 2729, -1, "", 1);// 190
            AddComplexComponent(this, 6077, -3, -2, 0, 2729, -1, "", 1);// 191
            AddComplexComponent(this, 6077, -4, 2, 0, 2729, -1, "", 1);// 192
            AddComplexComponent(this, 6077, -4, 1, 0, 2729, -1, "", 1);// 193
            AddComplexComponent(this, 6077, -4, 0, 0, 2729, -1, "", 1);// 194
            AddComplexComponent(this, 6077, -4, -1, 0, 2729, -1, "", 1);// 195
            AddComplexComponent(this, 6077, -4, -2, 0, 2729, -1, "", 1);// 196
            AddComplexComponent(this, 6077, -5, 2, 0, 2729, -1, "", 1);// 197
            AddComplexComponent(this, 6077, -5, 1, 0, 2729, -1, "", 1);// 198
            AddComplexComponent(this, 6077, -5, 0, 0, 2729, -1, "", 1);// 199
            AddComplexComponent(this, 6077, -5, -1, 0, 2729, -1, "", 1);// 200
            AddComplexComponent(this, 6077, -5, -2, 0, 2729, -1, "", 1);// 201
            AddComplexComponent(this, 6077, -6, 2, 0, 2729, -1, "", 1);// 202
            AddComplexComponent(this, 6077, -6, 1, 0, 2729, -1, "", 1);// 203
            AddComplexComponent(this, 6077, -6, 0, 0, 2729, -1, "", 1);// 204
            AddComplexComponent(this, 6077, -6, -1, 0, 2729, -1, "", 1);// 205
            AddComplexComponent(this, 6077, -6, -2, 0, 2729, -1, "", 1);// 206
            AddComplexComponent(this, 6077, 1, -3, 0, 2729, -1, "", 1);// 207
            AddComplexComponent(this, 6077, 1, -4, 0, 2729, -1, "", 1);// 208
            AddComplexComponent(this, 6077, 1, -5, 0, 2729, -1, "", 1);// 209
            AddComplexComponent(this, 6077, 0, -3, 0, 2729, -1, "", 1);// 210
            AddComplexComponent(this, 6077, 0, -4, 0, 2729, -1, "", 1);// 211
            AddComplexComponent(this, 6077, 0, -5, 0, 2729, -1, "", 1);// 212
            AddComplexComponent(this, 6077, -1, -3, 0, 2729, -1, "", 1);// 213
            AddComplexComponent(this, 6077, -1, -4, 0, 2729, -1, "", 1);// 214
            AddComplexComponent(this, 6077, -1, -5, 0, 2729, -1, "", 1);// 215
            AddComplexComponent(this, 6077, -2, -3, 0, 2729, -1, "", 1);// 216
            AddComplexComponent(this, 6077, -2, -4, 0, 2729, -1, "", 1);// 217
            AddComplexComponent(this, 6077, -2, -5, 0, 2729, -1, "", 1);// 218
            AddComplexComponent(this, 6077, -3, -3, 0, 2729, -1, "", 1);// 219
            AddComplexComponent(this, 6077, -3, -4, 0, 2729, -1, "", 1);// 220
            AddComplexComponent(this, 6077, -3, -5, 0, 2729, -1, "", 1);// 221
            AddComplexComponent(this, 6077, -4, -3, 0, 2729, -1, "", 1);// 222
            AddComplexComponent(this, 6077, -4, -4, 0, 2729, -1, "", 1);// 223
            AddComplexComponent(this, 6077, -4, -5, 0, 2729, -1, "", 1);// 224
            AddComplexComponent(this, 6077, -5, -3, 0, 2729, -1, "", 1);// 225
            AddComplexComponent(this, 6077, -5, -4, 0, 2729, -1, "", 1);// 226
            AddComplexComponent(this, 6077, -5, -5, 0, 2729, -1, "", 1);// 227
            AddComplexComponent(this, 6077, -6, -3, 0, 2729, -1, "", 1);// 228
            AddComplexComponent(this, 6077, -6, -4, 0, 2729, -1, "", 1);// 229
            AddComplexComponent(this, 6077, -6, -5, 0, 2729, -1, "", 1);// 230
            AddComplexComponent(this, 6077, 9, -2, 0, 2729, -1, "", 1);// 231
            AddComplexComponent(this, 6077, 9, -3, 0, 2729, -1, "", 1);// 232
            AddComplexComponent(this, 6077, 9, -4, 0, 2729, -1, "", 1);// 233
            AddComplexComponent(this, 6077, 9, -5, 0, 2729, -1, "", 1);// 234
            AddComplexComponent(this, 6077, 8, -2, 0, 2729, -1, "", 1);// 235
            AddComplexComponent(this, 6077, 8, -3, 0, 2729, -1, "", 1);// 236
            AddComplexComponent(this, 6077, 8, -4, 0, 2729, -1, "", 1);// 237
            AddComplexComponent(this, 6077, 8, -5, 0, 2729, -1, "", 1);// 238
            AddComplexComponent(this, 6077, 7, -2, 0, 2729, -1, "", 1);// 239
            AddComplexComponent(this, 6077, 7, -3, 0, 2729, -1, "", 1);// 240
            AddComplexComponent(this, 6077, 7, -4, 0, 2729, -1, "", 1);// 241
            AddComplexComponent(this, 6077, 7, -5, 0, 2729, -1, "", 1);// 242
            AddComplexComponent(this, 6077, 6, -2, 0, 2729, -1, "", 1);// 243
            AddComplexComponent(this, 6077, 6, -3, 0, 2729, -1, "", 1);// 244
            AddComplexComponent(this, 6077, 6, -4, 0, 2729, -1, "", 1);// 245
            AddComplexComponent(this, 6077, 6, -5, 0, 2729, -1, "", 1);// 246
            AddComplexComponent(this, 6077, 5, -2, 0, 2729, -1, "", 1);// 247
            AddComplexComponent(this, 6077, 5, -3, 0, 2729, -1, "", 1);// 248
            AddComplexComponent(this, 6077, 5, -4, 0, 2729, -1, "", 1);// 249
            AddComplexComponent(this, 6077, 5, -5, 0, 2729, -1, "", 1);// 250
            AddComplexComponent(this, 6077, 4, -2, 0, 2729, -1, "", 1);// 251
            AddComplexComponent(this, 6077, 4, -3, 0, 2729, -1, "", 1);// 252
            AddComplexComponent(this, 6077, 4, -4, 0, 2729, -1, "", 1);// 253
            AddComplexComponent(this, 6077, 4, -5, 0, 2729, -1, "", 1);// 254
            AddComplexComponent(this, 6077, 3, -2, 0, 2729, -1, "", 1);// 255
            AddComplexComponent(this, 6077, 3, -3, 0, 2729, -1, "", 1);// 256
            AddComplexComponent(this, 6077, 3, -4, 0, 2729, -1, "", 1);// 257
            AddComplexComponent(this, 6077, 3, -5, 0, 2729, -1, "", 1);// 258
            AddComplexComponent(this, 6077, 2, -2, 0, 2729, -1, "", 1);// 259
            AddComplexComponent(this, 6077, 2, -3, 0, 2729, -1, "", 1);// 260
            AddComplexComponent(this, 6077, 2, -4, 0, 2729, -1, "", 1);// 261
            AddComplexComponent(this, 6077, 2, -5, 0, 2729, -1, "", 1);// 262
            AddComplexComponent(this, 6077, 1, -2, 0, 2729, -1, "", 1);// 263
            AddComplexComponent(this, 17780, -4, -9, 0, 1152, -1, "", 1);// 264
            AddComplexComponent(this, 17780, 1, -9, 0, 1152, -1, "", 1);// 265
            AddComplexComponent(this, 17780, 9, -9, 0, 1152, -1, "", 1);// 266
            AddComplexComponent(this, 17780, 8, -9, 0, 1152, -1, "", 1);// 267
            AddComplexComponent(this, 17780, 8, -9, 0, 1152, -1, "", 1);// 268
            AddComplexComponent(this, 6257, -1, -9, 5, 2729, -1, "", 1);// 269
            AddComplexComponent(this, 17779, -5, -9, 0, 1152, -1, "", 1);// 270
            AddComplexComponent(this, 17779, -2, -9, 0, 1152, -1, "", 1);// 271
            AddComplexComponent(this, 17779, 2, -9, 0, 1152, -1, "", 1);// 272
            AddComplexComponent(this, 17779, 7, -9, 0, 1152, -1, "", 1);// 273
            AddComplexComponent(this, 17778, -6, -9, 0, 1152, -1, "", 1);// 274
            AddComplexComponent(this, 17778, -3, -9, 0, 1152, -1, "", 1);// 275
            AddComplexComponent(this, 17778, 0, -9, 0, 1152, -1, "", 1);// 276
            AddComplexComponent(this, 17778, 3, -9, 0, 1152, -1, "", 1);// 277
            AddComplexComponent(this, 17778, 4, -9, 0, 1152, -1, "", 1);// 278
            AddComplexComponent(this, 17778, 5, -9, 0, 1152, -1, "", 1);// 279
            AddComplexComponent(this, 17778, 6, -9, 0, 1152, -1, "", 1);// 280
            AddComplexComponent(this, 22300, 5, -7, 0, 1152, -1, "", 1);// 289
            AddComplexComponent(this, 6077, 1, -6, 0, 2729, -1, "", 1);// 290
            AddComplexComponent(this, 6077, 1, -7, 0, 2729, -1, "", 1);// 291
            AddComplexComponent(this, 6077, 1, -8, 0, 2729, -1, "", 1);// 292
            AddComplexComponent(this, 6077, 0, -6, 0, 2729, -1, "", 1);// 293
            AddComplexComponent(this, 6077, 0, -7, 0, 2729, -1, "", 1);// 294
            AddComplexComponent(this, 6077, 0, -8, 0, 2729, -1, "", 1);// 295
            AddComplexComponent(this, 6077, -1, -6, 0, 2729, -1, "", 1);// 296
            AddComplexComponent(this, 6077, -1, -7, 0, 2729, -1, "", 1);// 297
            AddComplexComponent(this, 6077, -1, -8, 0, 2729, -1, "", 1);// 298
            AddComplexComponent(this, 6077, -2, -6, 0, 2729, -1, "", 1);// 299
            AddComplexComponent(this, 6077, -2, -7, 0, 2729, -1, "", 1);// 300
            AddComplexComponent(this, 6077, -2, -8, 0, 2729, -1, "", 1);// 301
            AddComplexComponent(this, 6077, -3, -6, 0, 2729, -1, "", 1);// 302
            AddComplexComponent(this, 6077, -3, -7, 0, 2729, -1, "", 1);// 303
            AddComplexComponent(this, 6077, -3, -8, 0, 2729, -1, "", 1);// 304
            AddComplexComponent(this, 6077, -4, -6, 0, 2729, -1, "", 1);// 305
            AddComplexComponent(this, 6077, -4, -7, 0, 2729, -1, "", 1);// 306
            AddComplexComponent(this, 6077, -4, -8, 0, 2729, -1, "", 1);// 307
            AddComplexComponent(this, 6077, -5, -6, 0, 2729, -1, "", 1);// 308
            AddComplexComponent(this, 6077, -5, -7, 0, 2729, -1, "", 1);// 309
            AddComplexComponent(this, 6077, -5, -8, 0, 2729, -1, "", 1);// 310
            AddComplexComponent(this, 6077, -6, -6, 0, 2729, -1, "", 1);// 311
            AddComplexComponent(this, 6077, -6, -7, 0, 2729, -1, "", 1);// 312
            AddComplexComponent(this, 6077, -6, -8, 0, 2729, -1, "", 1);// 313
            AddComplexComponent(this, 6077, 9, -8, 0, 2729, -1, "", 1);// 314
            AddComplexComponent(this, 6077, 9, -9, 0, 2729, -1, "", 1);// 315
            AddComplexComponent(this, 6077, 8, -8, 0, 2729, -1, "", 1);// 316
            AddComplexComponent(this, 6077, 8, -9, 0, 2729, -1, "", 1);// 317
            AddComplexComponent(this, 6077, 7, -8, 0, 2729, -1, "", 1);// 318
            AddComplexComponent(this, 6077, 7, -9, 0, 2729, -1, "", 1);// 319
            AddComplexComponent(this, 6077, 6, -8, 0, 2729, -1, "", 1);// 320
            AddComplexComponent(this, 6077, 6, -9, 0, 2729, -1, "", 1);// 321
            AddComplexComponent(this, 6077, 5, -8, 0, 2729, -1, "", 1);// 322
            AddComplexComponent(this, 6077, 5, -9, 0, 2729, -1, "", 1);// 323
            AddComplexComponent(this, 6077, 4, -8, 0, 2729, -1, "", 1);// 324
            AddComplexComponent(this, 6077, 4, -9, 0, 2729, -1, "", 1);// 325
            AddComplexComponent(this, 6077, 3, -8, 0, 2729, -1, "", 1);// 326
            AddComplexComponent(this, 6077, 3, -9, 0, 2729, -1, "", 1);// 327
            AddComplexComponent(this, 6077, 2, -8, 0, 2729, -1, "", 1);// 328
            AddComplexComponent(this, 6077, 2, -9, 0, 2729, -1, "", 1);// 329
            AddComplexComponent(this, 6077, 9, -6, 0, 2729, -1, "", 1);// 330
            AddComplexComponent(this, 6077, 9, -7, 0, 2729, -1, "", 1);// 331
            AddComplexComponent(this, 6077, 8, -6, 0, 2729, -1, "", 1);// 332
            AddComplexComponent(this, 6077, 8, -7, 0, 2729, -1, "", 1);// 333
            AddComplexComponent(this, 6077, 7, -6, 0, 2729, -1, "", 1);// 334
            AddComplexComponent(this, 6077, 7, -7, 0, 2729, -1, "", 1);// 335
            AddComplexComponent(this, 6077, 6, -6, 0, 2729, -1, "", 1);// 336
            AddComplexComponent(this, 6077, 6, -7, 0, 2729, -1, "", 1);// 337
            AddComplexComponent(this, 6077, 5, -6, 0, 2729, -1, "", 1);// 338
            AddComplexComponent(this, 6077, 5, -7, 0, 2729, -1, "", 1);// 339
            AddComplexComponent(this, 6077, 4, -6, 0, 2729, -1, "", 1);// 340
            AddComplexComponent(this, 6077, 4, -7, 0, 2729, -1, "", 1);// 341
            AddComplexComponent(this, 6077, 3, -6, 0, 2729, -1, "", 1);// 342
            AddComplexComponent(this, 6077, 3, -7, 0, 2729, -1, "", 1);// 343
            AddComplexComponent(this, 6077, 2, -6, 0, 2729, -1, "", 1);// 344
            AddComplexComponent(this, 6077, 2, -7, 0, 2729, -1, "", 1);// 345
            AddComplexComponent(this, 6077, 1, -9, 0, 2729, -1, "", 1);// 346
            AddComplexComponent(this, 6077, 0, -9, 0, 2729, -1, "", 1);// 347
            AddComplexComponent(this, 6077, -1, -9, 0, 2729, -1, "", 1);// 348
            AddComplexComponent(this, 6077, -2, -9, 0, 2729, -1, "", 1);// 349
            AddComplexComponent(this, 6077, -3, -9, 0, 2729, -1, "", 1);// 350
            AddComplexComponent(this, 6077, -4, -9, 0, 2729, -1, "", 1);// 351
            AddComplexComponent(this, 6077, -5, -9, 0, 2729, -1, "", 1);// 352
            AddComplexComponent(this, 6077, -6, -9, 0, 2729, -1, "", 1);// 353
            AddComplexComponent(this, 17754, -8, 5, 0, 2729, -1, "", 1);// 354
            AddComplexComponent(this, 17754, -9, 5, 0, 2729, -1, "", 1);// 355
            AddComplexComponent(this, 17760, -10, 5, 0, 2729, -1, "", 1);// 356
            AddComplexComponent(this, 17758, -11, 5, 0, 2729, -1, "", 1);// 357
            AddComplexComponent(this, 17754, -12, 5, 0, 2729, -1, "", 1);// 358
            AddComplexComponent(this, 17756, -12, 4, 0, 2729, -1, "", 1);// 359
            AddComplexComponent(this, 17724, -11, 4, 0, 2729, -1, "", 1);// 360
            AddComplexComponent(this, 17757, -10, 4, 0, 2729, -1, "", 1);// 361
            AddComplexComponent(this, 17729, -9, 4, 0, 2729, -1, "", 1);// 362
            AddComplexComponent(this, 17782, -13, -3, 0, 1152, -1, "", 1);// 363
            AddComplexComponent(this, 17782, -13, -2, 0, 1152, -1, "", 1);// 364
            AddComplexComponent(this, 17782, -13, 0, 0, 1152, -1, "", 1);// 365
            AddComplexComponent(this, 17783, -13, 1, 0, 1152, -1, "", 1);// 366
            AddComplexComponent(this, 17783, -13, -1, 0, 1152, -1, "", 1);// 367
            AddComplexComponent(this, 17783, -13, -4, 0, 1152, -1, "", 1);// 368
            AddComplexComponent(this, 17781, -13, 2, 0, 1152, -1, "", 1);// 369
            AddComplexComponent(this, 17781, -13, -5, 0, 1152, -1, "", 1);// 370
            AddComplexComponent(this, 17734, -8, 4, 0, 2729, -1, "", 1);// 371
            AddComplexComponent(this, 19122, -9, -1, 0, 1152, -1, "", 1);// 372
            AddComplexComponent(this, 22300, -11, 7, 0, 1152, -1, "", 1);// 387
            AddComplexComponent(this, 6077, -8, 8, 0, 2729, -1, "", 1);// 388
            AddComplexComponent(this, 6077, -8, 7, 0, 2729, -1, "", 1);// 389
            AddComplexComponent(this, 6077, -8, 6, 0, 2729, -1, "", 1);// 390
            AddComplexComponent(this, 6077, -8, 5, 0, 2729, -1, "", 1);// 391
            AddComplexComponent(this, 6077, -8, 4, 0, 2729, -1, "", 1);// 392
            AddComplexComponent(this, 6077, -8, 3, 0, 2729, -1, "", 1);// 393
            AddComplexComponent(this, 6077, -9, 8, 0, 2729, -1, "", 1);// 394
            AddComplexComponent(this, 6077, -9, 7, 0, 2729, -1, "", 1);// 395
            AddComplexComponent(this, 6077, -9, 6, 0, 2729, -1, "", 1);// 396
            AddComplexComponent(this, 6077, -9, 5, 0, 2729, -1, "", 1);// 397
            AddComplexComponent(this, 6077, -9, 4, 0, 2729, -1, "", 1);// 398
            AddComplexComponent(this, 6077, -9, 3, 0, 2729, -1, "", 1);// 399
            AddComplexComponent(this, 6077, -10, 8, 0, 2729, -1, "", 1);// 400
            AddComplexComponent(this, 6077, -10, 7, 0, 2729, -1, "", 1);// 401
            AddComplexComponent(this, 6077, -10, 6, 0, 2729, -1, "", 1);// 402
            AddComplexComponent(this, 6077, -10, 5, 0, 2729, -1, "", 1);// 403
            AddComplexComponent(this, 6077, -10, 4, 0, 2729, -1, "", 1);// 404
            AddComplexComponent(this, 6077, -10, 3, 0, 2729, -1, "", 1);// 405
            AddComplexComponent(this, 6077, -11, 8, 0, 2729, -1, "", 1);// 406
            AddComplexComponent(this, 6077, -11, 7, 0, 2729, -1, "", 1);// 407
            AddComplexComponent(this, 6077, -11, 6, 0, 2729, -1, "", 1);// 408
            AddComplexComponent(this, 6077, -11, 5, 0, 2729, -1, "", 1);// 409
            AddComplexComponent(this, 6077, -11, 4, 0, 2729, -1, "", 1);// 410
            AddComplexComponent(this, 6077, -11, 3, 0, 2729, -1, "", 1);// 411
            AddComplexComponent(this, 6077, -7, 2, 0, 2729, -1, "", 1);// 412
            AddComplexComponent(this, 6077, -7, 1, 0, 2729, -1, "", 1);// 413
            AddComplexComponent(this, 6077, -7, 0, 0, 2729, -1, "", 1);// 414
            AddComplexComponent(this, 6077, -7, -1, 0, 2729, -1, "", 1);// 415
            AddComplexComponent(this, 6077, -7, -2, 0, 2729, -1, "", 1);// 416
            AddComplexComponent(this, 6077, -8, 2, 0, 2729, -1, "", 1);// 417
            AddComplexComponent(this, 6077, -8, 1, 0, 2729, -1, "", 1);// 418
            AddComplexComponent(this, 6077, -8, 0, 0, 2729, -1, "", 1);// 419
            AddComplexComponent(this, 6077, -8, -1, 0, 2729, -1, "", 1);// 420
            AddComplexComponent(this, 6077, -8, -2, 0, 2729, -1, "", 1);// 421
            AddComplexComponent(this, 6077, -9, 2, 0, 2729, -1, "", 1);// 422
            AddComplexComponent(this, 6077, -9, 1, 0, 2729, -1, "", 1);// 423
            AddComplexComponent(this, 6077, -9, 0, 0, 2729, -1, "", 1);// 424
            AddComplexComponent(this, 6077, -9, -1, 0, 2729, -1, "", 1);// 425
            AddComplexComponent(this, 6077, -9, -2, 0, 2729, -1, "", 1);// 426
            AddComplexComponent(this, 6077, -10, 2, 0, 2729, -1, "", 1);// 427
            AddComplexComponent(this, 6077, -10, 1, 0, 2729, -1, "", 1);// 428
            AddComplexComponent(this, 6077, -10, 0, 0, 2729, -1, "", 1);// 429
            AddComplexComponent(this, 6077, -10, -1, 0, 2729, -1, "", 1);// 430
            AddComplexComponent(this, 6077, -10, -2, 0, 2729, -1, "", 1);// 431
            AddComplexComponent(this, 6077, -11, 2, 0, 2729, -1, "", 1);// 432
            AddComplexComponent(this, 6077, -11, 1, 0, 2729, -1, "", 1);// 433
            AddComplexComponent(this, 6077, -11, 0, 0, 2729, -1, "", 1);// 434
            AddComplexComponent(this, 6077, -11, -1, 0, 2729, -1, "", 1);// 435
            AddComplexComponent(this, 6077, -11, -2, 0, 2729, -1, "", 1);// 436
            AddComplexComponent(this, 6077, -12, 2, 0, 2729, -1, "", 1);// 437
            AddComplexComponent(this, 6077, -12, 1, 0, 2729, -1, "", 1);// 438
            AddComplexComponent(this, 6077, -12, 0, 0, 2729, -1, "", 1);// 439
            AddComplexComponent(this, 6077, -12, -1, 0, 2729, -1, "", 1);// 440
            AddComplexComponent(this, 6077, -12, -2, 0, 2729, -1, "", 1);// 441
            AddComplexComponent(this, 6077, -7, -3, 0, 2729, -1, "", 1);// 442
            AddComplexComponent(this, 6077, -7, -4, 0, 2729, -1, "", 1);// 443
            AddComplexComponent(this, 6077, -7, -5, 0, 2729, -1, "", 1);// 444
            AddComplexComponent(this, 6077, -8, -3, 0, 2729, -1, "", 1);// 445
            AddComplexComponent(this, 6077, -8, -4, 0, 2729, -1, "", 1);// 446
            AddComplexComponent(this, 6077, -8, -5, 0, 2729, -1, "", 1);// 447
            AddComplexComponent(this, 6077, -9, -3, 0, 2729, -1, "", 1);// 448
            AddComplexComponent(this, 6077, -9, -4, 0, 2729, -1, "", 1);// 449
            AddComplexComponent(this, 6077, -9, -5, 0, 2729, -1, "", 1);// 450
            AddComplexComponent(this, 6077, -10, -3, 0, 2729, -1, "", 1);// 451
            AddComplexComponent(this, 6077, -10, -4, 0, 2729, -1, "", 1);// 452
            AddComplexComponent(this, 6077, -10, -5, 0, 2729, -1, "", 1);// 453
            AddComplexComponent(this, 6077, -11, -3, 0, 2729, -1, "", 1);// 454
            AddComplexComponent(this, 6077, -11, -4, 0, 2729, -1, "", 1);// 455
            AddComplexComponent(this, 6077, -11, -5, 0, 2729, -1, "", 1);// 456
            AddComplexComponent(this, 6077, -12, -3, 0, 2729, -1, "", 1);// 457
            AddComplexComponent(this, 6077, -12, -4, 0, 2729, -1, "", 1);// 458
            AddComplexComponent(this, 6077, -12, -5, 0, 2729, -1, "", 1);// 459
            AddComplexComponent(this, 6077, -13, -5, 0, 2729, -1, "", 1);// 460
            AddComplexComponent(this, 6077, -13, -4, 0, 2729, -1, "", 1);// 461
            AddComplexComponent(this, 6077, -13, -3, 0, 2729, -1, "", 1);// 462
            AddComplexComponent(this, 6077, -13, -2, 0, 2729, -1, "", 1);// 463
            AddComplexComponent(this, 6077, -13, -1, 0, 2729, -1, "", 1);// 464
            AddComplexComponent(this, 6077, -13, 0, 0, 2729, -1, "", 1);// 465
            AddComplexComponent(this, 6077, -13, 1, 0, 2729, -1, "", 1);// 466
            AddComplexComponent(this, 6077, -13, 2, 0, 2729, -1, "", 1);// 467
            AddComplexComponent(this, 6077, -13, 3, 0, 2729, -1, "", 1);// 468
            AddComplexComponent(this, 6077, -12, 3, 0, 2729, -1, "", 1);// 469
            AddComplexComponent(this, 6077, -12, 4, 0, 2729, -1, "", 1);// 470
            AddComplexComponent(this, 6077, -12, 5, 0, 2729, -1, "", 1);// 471
            AddComplexComponent(this, 6077, -12, 6, 0, 2729, -1, "", 1);// 472
            AddComplexComponent(this, 6077, -12, 7, 0, 2729, -1, "", 1);// 473
            AddComplexComponent(this, 6077, -12, 8, 0, 2729, -1, "", 1);// 474
            AddComplexComponent(this, 17306, -8, 9, 0, 2729, -1, "", 1);// 475
            AddComplexComponent(this, 17306, -9, 9, 0, 2729, -1, "", 1);// 476
            AddComplexComponent(this, 17306, -10, 9, 0, 2729, -1, "", 1);// 477
            AddComplexComponent(this, 17306, -11, 9, 0, 2729, -1, "", 1);// 478
            AddComplexComponent(this, 17306, -12, 9, 0, 2729, -1, "", 1);// 479
            AddComplexComponent(this, 17783, -13, -6, 0, 1152, -1, "", 1);// 480
            AddComplexComponent(this, 17783, -13, -7, 0, 1152, -1, "", 1);// 481
            AddComplexComponent(this, 17781, -13, -8, 0, 1152, -1, "", 1);// 482
            AddComplexComponent(this, 17780, -9, -9, 0, 1152, -1, "", 1);// 483
            AddComplexComponent(this, 17780, -7, -9, 0, 1152, -1, "", 1);// 484
            AddComplexComponent(this, 17779, -11, -9, 0, 1152, -1, "", 1);// 485
            AddComplexComponent(this, 17779, -12, -9, 0, 1152, -1, "", 1);// 486
            AddComplexComponent(this, 17779, -8, -9, 0, 1152, -1, "", 1);// 487
            AddComplexComponent(this, 17778, -10, -9, 0, 1152, -1, "", 1);// 488
            AddComplexComponent(this, 22300, -12, -7, 0, 1152, -1, "", 1);// 493
            AddComplexComponent(this, 6077, -7, -6, 0, 2729, -1, "", 1);// 494
            AddComplexComponent(this, 6077, -7, -7, 0, 2729, -1, "", 1);// 495
            AddComplexComponent(this, 6077, -7, -8, 0, 2729, -1, "", 1);// 496
            AddComplexComponent(this, 6077, -8, -6, 0, 2729, -1, "", 1);// 497
            AddComplexComponent(this, 6077, -8, -7, 0, 2729, -1, "", 1);// 498
            AddComplexComponent(this, 6077, -8, -8, 0, 2729, -1, "", 1);// 499
            AddComplexComponent(this, 6077, -9, -6, 0, 2729, -1, "", 1);// 500
            AddComplexComponent(this, 6077, -9, -7, 0, 2729, -1, "", 1);// 501
            AddComplexComponent(this, 6077, -9, -8, 0, 2729, -1, "", 1);// 502
            AddComplexComponent(this, 6077, -10, -6, 0, 2729, -1, "", 1);// 503
            AddComplexComponent(this, 6077, -10, -7, 0, 2729, -1, "", 1);// 504
            AddComplexComponent(this, 6077, -10, -8, 0, 2729, -1, "", 1);// 505
            AddComplexComponent(this, 6077, -11, -6, 0, 2729, -1, "", 1);// 506
            AddComplexComponent(this, 6077, -11, -7, 0, 2729, -1, "", 1);// 507
            AddComplexComponent(this, 6077, -11, -8, 0, 2729, -1, "", 1);// 508
            AddComplexComponent(this, 6077, -12, -6, 0, 2729, -1, "", 1);// 509
            AddComplexComponent(this, 6077, -12, -7, 0, 2729, -1, "", 1);// 510
            AddComplexComponent(this, 6077, -12, -8, 0, 2729, -1, "", 1);// 511
            AddComplexComponent(this, 6077, -7, -9, 0, 2729, -1, "", 1);// 512
            AddComplexComponent(this, 6077, -8, -9, 0, 2729, -1, "", 1);// 513
            AddComplexComponent(this, 6077, -9, -9, 0, 2729, -1, "", 1);// 514
            AddComplexComponent(this, 6077, -10, -9, 0, 2729, -1, "", 1);// 515
            AddComplexComponent(this, 6077, -11, -9, 0, 2729, -1, "", 1);// 516
            AddComplexComponent(this, 6077, -12, -9, 0, 2729, -1, "", 1);// 517
            AddComplexComponent(this, 6077, -13, -9, 0, 2729, -1, "", 1);// 518
            AddComplexComponent(this, 6077, -13, -8, 0, 2729, -1, "", 1);// 519
            AddComplexComponent(this, 6077, -13, -7, 0, 2729, -1, "", 1);// 520
            AddComplexComponent(this, 6077, -13, -6, 0, 2729, -1, "", 1);// 521
        }

        public BlackthornStep3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

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
