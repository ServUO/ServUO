using System;

namespace Server.Items
{
    public class BlackthornStep3 : BlackthornBaseAddon
    {
        public static BlackthornStep3 InstanceTram { get; set; }
        public static BlackthornStep3 InstanceFel { get; set; }

        private static int[,] m_AddOnSimpleComponents = new int[,]
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

            AddComplexComponent((BaseAddon)this, 6077, 12, 8, 0, 2729, -1, "", 1);// 5
            AddComplexComponent((BaseAddon)this, 6077, 12, 7, 0, 2729, -1, "", 1);// 6
            AddComplexComponent((BaseAddon)this, 6077, 12, 6, 0, 2729, -1, "", 1);// 7
            AddComplexComponent((BaseAddon)this, 6077, 12, 5, 0, 2729, -1, "", 1);// 8
            AddComplexComponent((BaseAddon)this, 6077, 12, 4, 0, 2729, -1, "", 1);// 9
            AddComplexComponent((BaseAddon)this, 6077, 12, 3, 0, 2729, -1, "", 1);// 10
            AddComplexComponent((BaseAddon)this, 6077, 11, 8, 0, 2729, -1, "", 1);// 11
            AddComplexComponent((BaseAddon)this, 6077, 11, 7, 0, 2729, -1, "", 1);// 12
            AddComplexComponent((BaseAddon)this, 6077, 11, 6, 0, 2729, -1, "", 1);// 13
            AddComplexComponent((BaseAddon)this, 6077, 11, 5, 0, 2729, -1, "", 1);// 14
            AddComplexComponent((BaseAddon)this, 6077, 11, 4, 0, 2729, -1, "", 1);// 15
            AddComplexComponent((BaseAddon)this, 6077, 11, 3, 0, 2729, -1, "", 1);// 16
            AddComplexComponent((BaseAddon)this, 6077, 10, 8, 0, 2729, -1, "", 1);// 17
            AddComplexComponent((BaseAddon)this, 6077, 10, 7, 0, 2729, -1, "", 1);// 18
            AddComplexComponent((BaseAddon)this, 6077, 10, 6, 0, 2729, -1, "", 1);// 19
            AddComplexComponent((BaseAddon)this, 6077, 10, 5, 0, 2729, -1, "", 1);// 20
            AddComplexComponent((BaseAddon)this, 6077, 10, 4, 0, 2729, -1, "", 1);// 21
            AddComplexComponent((BaseAddon)this, 6077, 10, 3, 0, 2729, -1, "", 1);// 22
            AddComplexComponent((BaseAddon)this, 6077, 13, 2, 0, 2729, -1, "", 1);// 23
            AddComplexComponent((BaseAddon)this, 6077, 13, 1, 0, 2729, -1, "", 1);// 24
            AddComplexComponent((BaseAddon)this, 6077, 13, 0, 0, 2729, -1, "", 1);// 25
            AddComplexComponent((BaseAddon)this, 6077, 13, -1, 0, 2729, -1, "", 1);// 26
            AddComplexComponent((BaseAddon)this, 6077, 12, 2, 0, 2729, -1, "", 1);// 27
            AddComplexComponent((BaseAddon)this, 6077, 12, 1, 0, 2729, -1, "", 1);// 28
            AddComplexComponent((BaseAddon)this, 6077, 12, 0, 0, 2729, -1, "", 1);// 29
            AddComplexComponent((BaseAddon)this, 6077, 12, -1, 0, 2729, -1, "", 1);// 30
            AddComplexComponent((BaseAddon)this, 6077, 11, 2, 0, 2729, -1, "", 1);// 31
            AddComplexComponent((BaseAddon)this, 6077, 11, 1, 0, 2729, -1, "", 1);// 32
            AddComplexComponent((BaseAddon)this, 6077, 11, 0, 0, 2729, -1, "", 1);// 33
            AddComplexComponent((BaseAddon)this, 6077, 11, -1, 0, 2729, -1, "", 1);// 34
            AddComplexComponent((BaseAddon)this, 6077, 10, 2, 0, 2729, -1, "", 1);// 35
            AddComplexComponent((BaseAddon)this, 6077, 10, 1, 0, 2729, -1, "", 1);// 36
            AddComplexComponent((BaseAddon)this, 6077, 10, 0, 0, 2729, -1, "", 1);// 37
            AddComplexComponent((BaseAddon)this, 6077, 10, -1, 0, 2729, -1, "", 1);// 38
            AddComplexComponent((BaseAddon)this, 6077, 13, -2, 0, 2729, -1, "", 1);// 39
            AddComplexComponent((BaseAddon)this, 6077, 13, -3, 0, 2729, -1, "", 1);// 40
            AddComplexComponent((BaseAddon)this, 6077, 13, -4, 0, 2729, -1, "", 1);// 41
            AddComplexComponent((BaseAddon)this, 6077, 13, -5, 0, 2729, -1, "", 1);// 42
            AddComplexComponent((BaseAddon)this, 6077, 12, -2, 0, 2729, -1, "", 1);// 43
            AddComplexComponent((BaseAddon)this, 6077, 12, -3, 0, 2729, -1, "", 1);// 44
            AddComplexComponent((BaseAddon)this, 6077, 12, -4, 0, 2729, -1, "", 1);// 45
            AddComplexComponent((BaseAddon)this, 6077, 12, -5, 0, 2729, -1, "", 1);// 46
            AddComplexComponent((BaseAddon)this, 6077, 11, -2, 0, 2729, -1, "", 1);// 47
            AddComplexComponent((BaseAddon)this, 6077, 11, -3, 0, 2729, -1, "", 1);// 48
            AddComplexComponent((BaseAddon)this, 6077, 11, -4, 0, 2729, -1, "", 1);// 49
            AddComplexComponent((BaseAddon)this, 6077, 11, -5, 0, 2729, -1, "", 1);// 50
            AddComplexComponent((BaseAddon)this, 6077, 10, -2, 0, 2729, -1, "", 1);// 51
            AddComplexComponent((BaseAddon)this, 6077, 10, -3, 0, 2729, -1, "", 1);// 52
            AddComplexComponent((BaseAddon)this, 6077, 10, -4, 0, 2729, -1, "", 1);// 53
            AddComplexComponent((BaseAddon)this, 6077, 10, -5, 0, 2729, -1, "", 1);// 54
            AddComplexComponent((BaseAddon)this, 7073, 12, -7, 0, 2729, -1, "", 1);// 56
            AddComplexComponent((BaseAddon)this, 7072, 11, -7, 0, 2729, -1, "", 1);// 57
            AddComplexComponent((BaseAddon)this, 7074, 10, -7, 0, 2729, -1, "", 1);// 58
            AddComplexComponent((BaseAddon)this, 7076, 10, -8, 0, 2729, -1, "", 1);// 59
            AddComplexComponent((BaseAddon)this, 7077, 10, -9, 0, 2729, -1, "", 1);// 60
            AddComplexComponent((BaseAddon)this, 7079, 11, -8, 0, 2729, -1, "", 1);// 61
            AddComplexComponent((BaseAddon)this, 7080, 12, -8, 0, 2729, -1, "", 1);// 62
            AddComplexComponent((BaseAddon)this, 7080, 11, -9, 0, 2729, -1, "", 1);// 63
            AddComplexComponent((BaseAddon)this, 7080, 12, -9, 0, 2729, -1, "", 1);// 64
            AddComplexComponent((BaseAddon)this, 17778, 11, -9, 0, 1152, -1, "", 1);// 65
            AddComplexComponent((BaseAddon)this, 17778, 12, -9, 0, 1152, -1, "", 1);// 66
            AddComplexComponent((BaseAddon)this, 6077, 13, -8, 0, 2729, -1, "", 1);// 69
            AddComplexComponent((BaseAddon)this, 6077, 13, -9, 0, 2729, -1, "", 1);// 70
            AddComplexComponent((BaseAddon)this, 6077, 12, -8, 0, 2729, -1, "", 1);// 71
            AddComplexComponent((BaseAddon)this, 6077, 12, -9, 0, 2729, -1, "", 1);// 72
            AddComplexComponent((BaseAddon)this, 6077, 11, -8, 0, 2729, -1, "", 1);// 73
            AddComplexComponent((BaseAddon)this, 6077, 11, -9, 0, 2729, -1, "", 1);// 74
            AddComplexComponent((BaseAddon)this, 6077, 10, -8, 0, 2729, -1, "", 1);// 75
            AddComplexComponent((BaseAddon)this, 6077, 10, -9, 0, 2729, -1, "", 1);// 76
            AddComplexComponent((BaseAddon)this, 6077, 13, -6, 0, 2729, -1, "", 1);// 77
            AddComplexComponent((BaseAddon)this, 6077, 13, -7, 0, 2729, -1, "", 1);// 78
            AddComplexComponent((BaseAddon)this, 6077, 12, -6, 0, 2729, -1, "", 1);// 79
            AddComplexComponent((BaseAddon)this, 6077, 12, -7, 0, 2729, -1, "", 1);// 80
            AddComplexComponent((BaseAddon)this, 6077, 11, -6, 0, 2729, -1, "", 1);// 81
            AddComplexComponent((BaseAddon)this, 6077, 11, -7, 0, 2729, -1, "", 1);// 82
            AddComplexComponent((BaseAddon)this, 6077, 10, -6, 0, 2729, -1, "", 1);// 83
            AddComplexComponent((BaseAddon)this, 6077, 10, -7, 0, 2729, -1, "", 1);// 84
            AddComplexComponent((BaseAddon)this, 7820, 4, -1, 0, 2729, -1, "", 1);// 85
            AddComplexComponent((BaseAddon)this, 7822, 3, -2, 0, 2729, -1, "", 1);// 86
            AddComplexComponent((BaseAddon)this, 7825, -2, -3, 0, 1152, -1, "", 1);// 87
            AddComplexComponent((BaseAddon)this, 22300, 9, 0, 0, 1152, -1, "", 1);// 113
            AddComplexComponent((BaseAddon)this, 22300, -6, -3, 0, 1152, -1, "", 1);// 114
            AddComplexComponent((BaseAddon)this, 22300, 1, -1, 0, 1152, -1, "", 1);// 115
            AddComplexComponent((BaseAddon)this, 7398, 8, 5, 0, 2729, -1, "", 1);// 116
            AddComplexComponent((BaseAddon)this, 6077, 6, 3, 0, 2729, -1, "", 1);// 117
            AddComplexComponent((BaseAddon)this, 6077, 9, 8, 0, 2729, -1, "", 1);// 118
            AddComplexComponent((BaseAddon)this, 6077, 9, 7, 0, 2729, -1, "", 1);// 119
            AddComplexComponent((BaseAddon)this, 6077, 9, 6, 0, 2729, -1, "", 1);// 120
            AddComplexComponent((BaseAddon)this, 6077, 9, 5, 0, 2729, -1, "", 1);// 121
            AddComplexComponent((BaseAddon)this, 6077, 9, 4, 0, 2729, -1, "", 1);// 122
            AddComplexComponent((BaseAddon)this, 6077, 9, 3, 0, 2729, -1, "", 1);// 123
            AddComplexComponent((BaseAddon)this, 6077, 8, 8, 0, 2729, -1, "", 1);// 124
            AddComplexComponent((BaseAddon)this, 6077, 8, 7, 0, 2729, -1, "", 1);// 125
            AddComplexComponent((BaseAddon)this, 6077, 8, 6, 0, 2729, -1, "", 1);// 126
            AddComplexComponent((BaseAddon)this, 6077, 8, 5, 0, 2729, -1, "", 1);// 127
            AddComplexComponent((BaseAddon)this, 6077, 8, 4, 0, 2729, -1, "", 1);// 128
            AddComplexComponent((BaseAddon)this, 6077, 8, 3, 0, 2729, -1, "", 1);// 129
            AddComplexComponent((BaseAddon)this, 6077, 7, 8, 0, 2729, -1, "", 1);// 130
            AddComplexComponent((BaseAddon)this, 6077, 7, 7, 0, 2729, -1, "", 1);// 131
            AddComplexComponent((BaseAddon)this, 6077, 7, 6, 0, 2729, -1, "", 1);// 132
            AddComplexComponent((BaseAddon)this, 6077, 7, 5, 0, 2729, -1, "", 1);// 133
            AddComplexComponent((BaseAddon)this, 6077, 7, 4, 0, 2729, -1, "", 1);// 134
            AddComplexComponent((BaseAddon)this, 6077, 7, 3, 0, 2729, -1, "", 1);// 135
            AddComplexComponent((BaseAddon)this, 6077, 9, 2, 0, 2729, -1, "", 1);// 136
            AddComplexComponent((BaseAddon)this, 6077, 9, 1, 0, 2729, -1, "", 1);// 137
            AddComplexComponent((BaseAddon)this, 6077, 9, 0, 0, 2729, -1, "", 1);// 138
            AddComplexComponent((BaseAddon)this, 6077, 9, -1, 0, 2729, -1, "", 1);// 139
            AddComplexComponent((BaseAddon)this, 6077, 8, 2, 0, 2729, -1, "", 1);// 140
            AddComplexComponent((BaseAddon)this, 6077, 8, 1, 0, 2729, -1, "", 1);// 141
            AddComplexComponent((BaseAddon)this, 6077, 8, 0, 0, 2729, -1, "", 1);// 142
            AddComplexComponent((BaseAddon)this, 6077, 8, -1, 0, 2729, -1, "", 1);// 143
            AddComplexComponent((BaseAddon)this, 6077, 7, 2, 0, 2729, -1, "", 1);// 144
            AddComplexComponent((BaseAddon)this, 6077, 7, 1, 0, 2729, -1, "", 1);// 145
            AddComplexComponent((BaseAddon)this, 6077, 7, 0, 0, 2729, -1, "", 1);// 146
            AddComplexComponent((BaseAddon)this, 6077, 7, -1, 0, 2729, -1, "", 1);// 147
            AddComplexComponent((BaseAddon)this, 6077, 6, 2, 0, 2729, -1, "", 1);// 148
            AddComplexComponent((BaseAddon)this, 6077, 6, 1, 0, 2729, -1, "", 1);// 149
            AddComplexComponent((BaseAddon)this, 6077, 6, 0, 0, 2729, -1, "", 1);// 150
            AddComplexComponent((BaseAddon)this, 6077, 6, -1, 0, 2729, -1, "", 1);// 151
            AddComplexComponent((BaseAddon)this, 6077, 5, 2, 0, 2729, -1, "", 1);// 152
            AddComplexComponent((BaseAddon)this, 6077, 5, 1, 0, 2729, -1, "", 1);// 153
            AddComplexComponent((BaseAddon)this, 6077, 5, 0, 0, 2729, -1, "", 1);// 154
            AddComplexComponent((BaseAddon)this, 6077, 5, -1, 0, 2729, -1, "", 1);// 155
            AddComplexComponent((BaseAddon)this, 6077, 4, 2, 0, 2729, -1, "", 1);// 156
            AddComplexComponent((BaseAddon)this, 6077, 4, 1, 0, 2729, -1, "", 1);// 157
            AddComplexComponent((BaseAddon)this, 6077, 4, 0, 0, 2729, -1, "", 1);// 158
            AddComplexComponent((BaseAddon)this, 6077, 4, -1, 0, 2729, -1, "", 1);// 159
            AddComplexComponent((BaseAddon)this, 6077, 3, 2, 0, 2729, -1, "", 1);// 160
            AddComplexComponent((BaseAddon)this, 6077, 3, 1, 0, 2729, -1, "", 1);// 161
            AddComplexComponent((BaseAddon)this, 6077, 3, 0, 0, 2729, -1, "", 1);// 162
            AddComplexComponent((BaseAddon)this, 6077, 3, -1, 0, 2729, -1, "", 1);// 163
            AddComplexComponent((BaseAddon)this, 6077, 2, 2, 0, 2729, -1, "", 1);// 164
            AddComplexComponent((BaseAddon)this, 6077, 2, 1, 0, 2729, -1, "", 1);// 165
            AddComplexComponent((BaseAddon)this, 6077, 2, 0, 0, 2729, -1, "", 1);// 166
            AddComplexComponent((BaseAddon)this, 6077, 2, -1, 0, 2729, -1, "", 1);// 167
            AddComplexComponent((BaseAddon)this, 6077, 1, 2, 0, 2729, -1, "", 1);// 168
            AddComplexComponent((BaseAddon)this, 6077, 1, 1, 0, 2729, -1, "", 1);// 169
            AddComplexComponent((BaseAddon)this, 6077, 1, 0, 0, 2729, -1, "", 1);// 170
            AddComplexComponent((BaseAddon)this, 6077, 1, -1, 0, 2729, -1, "", 1);// 171
            AddComplexComponent((BaseAddon)this, 6077, 0, 2, 0, 2729, -1, "", 1);// 172
            AddComplexComponent((BaseAddon)this, 6077, 0, 1, 0, 2729, -1, "", 1);// 173
            AddComplexComponent((BaseAddon)this, 6077, 0, 0, 0, 2729, -1, "", 1);// 174
            AddComplexComponent((BaseAddon)this, 6077, 0, -1, 0, 2729, -1, "", 1);// 175
            AddComplexComponent((BaseAddon)this, 6077, 0, -2, 0, 2729, -1, "", 1);// 176
            AddComplexComponent((BaseAddon)this, 6077, -1, 2, 0, 2729, -1, "", 1);// 177
            AddComplexComponent((BaseAddon)this, 6077, -1, 1, 0, 2729, -1, "", 1);// 178
            AddComplexComponent((BaseAddon)this, 6077, -1, 0, 0, 2729, -1, "", 1);// 179
            AddComplexComponent((BaseAddon)this, 6077, -1, -1, 0, 2729, -1, "", 1);// 180
            AddComplexComponent((BaseAddon)this, 6077, -1, -2, 0, 2729, -1, "", 1);// 181
            AddComplexComponent((BaseAddon)this, 6077, -2, 2, 0, 2729, -1, "", 1);// 182
            AddComplexComponent((BaseAddon)this, 6077, -2, 1, 0, 2729, -1, "", 1);// 183
            AddComplexComponent((BaseAddon)this, 6077, -2, 0, 0, 2729, -1, "", 1);// 184
            AddComplexComponent((BaseAddon)this, 6077, -2, -1, 0, 2729, -1, "", 1);// 185
            AddComplexComponent((BaseAddon)this, 6077, -2, -2, 0, 2729, -1, "", 1);// 186
            AddComplexComponent((BaseAddon)this, 6077, -3, 2, 0, 2729, -1, "", 1);// 187
            AddComplexComponent((BaseAddon)this, 6077, -3, 1, 0, 2729, -1, "", 1);// 188
            AddComplexComponent((BaseAddon)this, 6077, -3, 0, 0, 2729, -1, "", 1);// 189
            AddComplexComponent((BaseAddon)this, 6077, -3, -1, 0, 2729, -1, "", 1);// 190
            AddComplexComponent((BaseAddon)this, 6077, -3, -2, 0, 2729, -1, "", 1);// 191
            AddComplexComponent((BaseAddon)this, 6077, -4, 2, 0, 2729, -1, "", 1);// 192
            AddComplexComponent((BaseAddon)this, 6077, -4, 1, 0, 2729, -1, "", 1);// 193
            AddComplexComponent((BaseAddon)this, 6077, -4, 0, 0, 2729, -1, "", 1);// 194
            AddComplexComponent((BaseAddon)this, 6077, -4, -1, 0, 2729, -1, "", 1);// 195
            AddComplexComponent((BaseAddon)this, 6077, -4, -2, 0, 2729, -1, "", 1);// 196
            AddComplexComponent((BaseAddon)this, 6077, -5, 2, 0, 2729, -1, "", 1);// 197
            AddComplexComponent((BaseAddon)this, 6077, -5, 1, 0, 2729, -1, "", 1);// 198
            AddComplexComponent((BaseAddon)this, 6077, -5, 0, 0, 2729, -1, "", 1);// 199
            AddComplexComponent((BaseAddon)this, 6077, -5, -1, 0, 2729, -1, "", 1);// 200
            AddComplexComponent((BaseAddon)this, 6077, -5, -2, 0, 2729, -1, "", 1);// 201
            AddComplexComponent((BaseAddon)this, 6077, -6, 2, 0, 2729, -1, "", 1);// 202
            AddComplexComponent((BaseAddon)this, 6077, -6, 1, 0, 2729, -1, "", 1);// 203
            AddComplexComponent((BaseAddon)this, 6077, -6, 0, 0, 2729, -1, "", 1);// 204
            AddComplexComponent((BaseAddon)this, 6077, -6, -1, 0, 2729, -1, "", 1);// 205
            AddComplexComponent((BaseAddon)this, 6077, -6, -2, 0, 2729, -1, "", 1);// 206
            AddComplexComponent((BaseAddon)this, 6077, 1, -3, 0, 2729, -1, "", 1);// 207
            AddComplexComponent((BaseAddon)this, 6077, 1, -4, 0, 2729, -1, "", 1);// 208
            AddComplexComponent((BaseAddon)this, 6077, 1, -5, 0, 2729, -1, "", 1);// 209
            AddComplexComponent((BaseAddon)this, 6077, 0, -3, 0, 2729, -1, "", 1);// 210
            AddComplexComponent((BaseAddon)this, 6077, 0, -4, 0, 2729, -1, "", 1);// 211
            AddComplexComponent((BaseAddon)this, 6077, 0, -5, 0, 2729, -1, "", 1);// 212
            AddComplexComponent((BaseAddon)this, 6077, -1, -3, 0, 2729, -1, "", 1);// 213
            AddComplexComponent((BaseAddon)this, 6077, -1, -4, 0, 2729, -1, "", 1);// 214
            AddComplexComponent((BaseAddon)this, 6077, -1, -5, 0, 2729, -1, "", 1);// 215
            AddComplexComponent((BaseAddon)this, 6077, -2, -3, 0, 2729, -1, "", 1);// 216
            AddComplexComponent((BaseAddon)this, 6077, -2, -4, 0, 2729, -1, "", 1);// 217
            AddComplexComponent((BaseAddon)this, 6077, -2, -5, 0, 2729, -1, "", 1);// 218
            AddComplexComponent((BaseAddon)this, 6077, -3, -3, 0, 2729, -1, "", 1);// 219
            AddComplexComponent((BaseAddon)this, 6077, -3, -4, 0, 2729, -1, "", 1);// 220
            AddComplexComponent((BaseAddon)this, 6077, -3, -5, 0, 2729, -1, "", 1);// 221
            AddComplexComponent((BaseAddon)this, 6077, -4, -3, 0, 2729, -1, "", 1);// 222
            AddComplexComponent((BaseAddon)this, 6077, -4, -4, 0, 2729, -1, "", 1);// 223
            AddComplexComponent((BaseAddon)this, 6077, -4, -5, 0, 2729, -1, "", 1);// 224
            AddComplexComponent((BaseAddon)this, 6077, -5, -3, 0, 2729, -1, "", 1);// 225
            AddComplexComponent((BaseAddon)this, 6077, -5, -4, 0, 2729, -1, "", 1);// 226
            AddComplexComponent((BaseAddon)this, 6077, -5, -5, 0, 2729, -1, "", 1);// 227
            AddComplexComponent((BaseAddon)this, 6077, -6, -3, 0, 2729, -1, "", 1);// 228
            AddComplexComponent((BaseAddon)this, 6077, -6, -4, 0, 2729, -1, "", 1);// 229
            AddComplexComponent((BaseAddon)this, 6077, -6, -5, 0, 2729, -1, "", 1);// 230
            AddComplexComponent((BaseAddon)this, 6077, 9, -2, 0, 2729, -1, "", 1);// 231
            AddComplexComponent((BaseAddon)this, 6077, 9, -3, 0, 2729, -1, "", 1);// 232
            AddComplexComponent((BaseAddon)this, 6077, 9, -4, 0, 2729, -1, "", 1);// 233
            AddComplexComponent((BaseAddon)this, 6077, 9, -5, 0, 2729, -1, "", 1);// 234
            AddComplexComponent((BaseAddon)this, 6077, 8, -2, 0, 2729, -1, "", 1);// 235
            AddComplexComponent((BaseAddon)this, 6077, 8, -3, 0, 2729, -1, "", 1);// 236
            AddComplexComponent((BaseAddon)this, 6077, 8, -4, 0, 2729, -1, "", 1);// 237
            AddComplexComponent((BaseAddon)this, 6077, 8, -5, 0, 2729, -1, "", 1);// 238
            AddComplexComponent((BaseAddon)this, 6077, 7, -2, 0, 2729, -1, "", 1);// 239
            AddComplexComponent((BaseAddon)this, 6077, 7, -3, 0, 2729, -1, "", 1);// 240
            AddComplexComponent((BaseAddon)this, 6077, 7, -4, 0, 2729, -1, "", 1);// 241
            AddComplexComponent((BaseAddon)this, 6077, 7, -5, 0, 2729, -1, "", 1);// 242
            AddComplexComponent((BaseAddon)this, 6077, 6, -2, 0, 2729, -1, "", 1);// 243
            AddComplexComponent((BaseAddon)this, 6077, 6, -3, 0, 2729, -1, "", 1);// 244
            AddComplexComponent((BaseAddon)this, 6077, 6, -4, 0, 2729, -1, "", 1);// 245
            AddComplexComponent((BaseAddon)this, 6077, 6, -5, 0, 2729, -1, "", 1);// 246
            AddComplexComponent((BaseAddon)this, 6077, 5, -2, 0, 2729, -1, "", 1);// 247
            AddComplexComponent((BaseAddon)this, 6077, 5, -3, 0, 2729, -1, "", 1);// 248
            AddComplexComponent((BaseAddon)this, 6077, 5, -4, 0, 2729, -1, "", 1);// 249
            AddComplexComponent((BaseAddon)this, 6077, 5, -5, 0, 2729, -1, "", 1);// 250
            AddComplexComponent((BaseAddon)this, 6077, 4, -2, 0, 2729, -1, "", 1);// 251
            AddComplexComponent((BaseAddon)this, 6077, 4, -3, 0, 2729, -1, "", 1);// 252
            AddComplexComponent((BaseAddon)this, 6077, 4, -4, 0, 2729, -1, "", 1);// 253
            AddComplexComponent((BaseAddon)this, 6077, 4, -5, 0, 2729, -1, "", 1);// 254
            AddComplexComponent((BaseAddon)this, 6077, 3, -2, 0, 2729, -1, "", 1);// 255
            AddComplexComponent((BaseAddon)this, 6077, 3, -3, 0, 2729, -1, "", 1);// 256
            AddComplexComponent((BaseAddon)this, 6077, 3, -4, 0, 2729, -1, "", 1);// 257
            AddComplexComponent((BaseAddon)this, 6077, 3, -5, 0, 2729, -1, "", 1);// 258
            AddComplexComponent((BaseAddon)this, 6077, 2, -2, 0, 2729, -1, "", 1);// 259
            AddComplexComponent((BaseAddon)this, 6077, 2, -3, 0, 2729, -1, "", 1);// 260
            AddComplexComponent((BaseAddon)this, 6077, 2, -4, 0, 2729, -1, "", 1);// 261
            AddComplexComponent((BaseAddon)this, 6077, 2, -5, 0, 2729, -1, "", 1);// 262
            AddComplexComponent((BaseAddon)this, 6077, 1, -2, 0, 2729, -1, "", 1);// 263
            AddComplexComponent((BaseAddon)this, 17780, -4, -9, 0, 1152, -1, "", 1);// 264
            AddComplexComponent((BaseAddon)this, 17780, 1, -9, 0, 1152, -1, "", 1);// 265
            AddComplexComponent((BaseAddon)this, 17780, 9, -9, 0, 1152, -1, "", 1);// 266
            AddComplexComponent((BaseAddon)this, 17780, 8, -9, 0, 1152, -1, "", 1);// 267
            AddComplexComponent((BaseAddon)this, 17780, 8, -9, 0, 1152, -1, "", 1);// 268
            AddComplexComponent((BaseAddon)this, 6257, -1, -9, 5, 2729, -1, "", 1);// 269
            AddComplexComponent((BaseAddon)this, 17779, -5, -9, 0, 1152, -1, "", 1);// 270
            AddComplexComponent((BaseAddon)this, 17779, -2, -9, 0, 1152, -1, "", 1);// 271
            AddComplexComponent((BaseAddon)this, 17779, 2, -9, 0, 1152, -1, "", 1);// 272
            AddComplexComponent((BaseAddon)this, 17779, 7, -9, 0, 1152, -1, "", 1);// 273
            AddComplexComponent((BaseAddon)this, 17778, -6, -9, 0, 1152, -1, "", 1);// 274
            AddComplexComponent((BaseAddon)this, 17778, -3, -9, 0, 1152, -1, "", 1);// 275
            AddComplexComponent((BaseAddon)this, 17778, 0, -9, 0, 1152, -1, "", 1);// 276
            AddComplexComponent((BaseAddon)this, 17778, 3, -9, 0, 1152, -1, "", 1);// 277
            AddComplexComponent((BaseAddon)this, 17778, 4, -9, 0, 1152, -1, "", 1);// 278
            AddComplexComponent((BaseAddon)this, 17778, 5, -9, 0, 1152, -1, "", 1);// 279
            AddComplexComponent((BaseAddon)this, 17778, 6, -9, 0, 1152, -1, "", 1);// 280
            AddComplexComponent((BaseAddon)this, 22300, 5, -7, 0, 1152, -1, "", 1);// 289
            AddComplexComponent((BaseAddon)this, 6077, 1, -6, 0, 2729, -1, "", 1);// 290
            AddComplexComponent((BaseAddon)this, 6077, 1, -7, 0, 2729, -1, "", 1);// 291
            AddComplexComponent((BaseAddon)this, 6077, 1, -8, 0, 2729, -1, "", 1);// 292
            AddComplexComponent((BaseAddon)this, 6077, 0, -6, 0, 2729, -1, "", 1);// 293
            AddComplexComponent((BaseAddon)this, 6077, 0, -7, 0, 2729, -1, "", 1);// 294
            AddComplexComponent((BaseAddon)this, 6077, 0, -8, 0, 2729, -1, "", 1);// 295
            AddComplexComponent((BaseAddon)this, 6077, -1, -6, 0, 2729, -1, "", 1);// 296
            AddComplexComponent((BaseAddon)this, 6077, -1, -7, 0, 2729, -1, "", 1);// 297
            AddComplexComponent((BaseAddon)this, 6077, -1, -8, 0, 2729, -1, "", 1);// 298
            AddComplexComponent((BaseAddon)this, 6077, -2, -6, 0, 2729, -1, "", 1);// 299
            AddComplexComponent((BaseAddon)this, 6077, -2, -7, 0, 2729, -1, "", 1);// 300
            AddComplexComponent((BaseAddon)this, 6077, -2, -8, 0, 2729, -1, "", 1);// 301
            AddComplexComponent((BaseAddon)this, 6077, -3, -6, 0, 2729, -1, "", 1);// 302
            AddComplexComponent((BaseAddon)this, 6077, -3, -7, 0, 2729, -1, "", 1);// 303
            AddComplexComponent((BaseAddon)this, 6077, -3, -8, 0, 2729, -1, "", 1);// 304
            AddComplexComponent((BaseAddon)this, 6077, -4, -6, 0, 2729, -1, "", 1);// 305
            AddComplexComponent((BaseAddon)this, 6077, -4, -7, 0, 2729, -1, "", 1);// 306
            AddComplexComponent((BaseAddon)this, 6077, -4, -8, 0, 2729, -1, "", 1);// 307
            AddComplexComponent((BaseAddon)this, 6077, -5, -6, 0, 2729, -1, "", 1);// 308
            AddComplexComponent((BaseAddon)this, 6077, -5, -7, 0, 2729, -1, "", 1);// 309
            AddComplexComponent((BaseAddon)this, 6077, -5, -8, 0, 2729, -1, "", 1);// 310
            AddComplexComponent((BaseAddon)this, 6077, -6, -6, 0, 2729, -1, "", 1);// 311
            AddComplexComponent((BaseAddon)this, 6077, -6, -7, 0, 2729, -1, "", 1);// 312
            AddComplexComponent((BaseAddon)this, 6077, -6, -8, 0, 2729, -1, "", 1);// 313
            AddComplexComponent((BaseAddon)this, 6077, 9, -8, 0, 2729, -1, "", 1);// 314
            AddComplexComponent((BaseAddon)this, 6077, 9, -9, 0, 2729, -1, "", 1);// 315
            AddComplexComponent((BaseAddon)this, 6077, 8, -8, 0, 2729, -1, "", 1);// 316
            AddComplexComponent((BaseAddon)this, 6077, 8, -9, 0, 2729, -1, "", 1);// 317
            AddComplexComponent((BaseAddon)this, 6077, 7, -8, 0, 2729, -1, "", 1);// 318
            AddComplexComponent((BaseAddon)this, 6077, 7, -9, 0, 2729, -1, "", 1);// 319
            AddComplexComponent((BaseAddon)this, 6077, 6, -8, 0, 2729, -1, "", 1);// 320
            AddComplexComponent((BaseAddon)this, 6077, 6, -9, 0, 2729, -1, "", 1);// 321
            AddComplexComponent((BaseAddon)this, 6077, 5, -8, 0, 2729, -1, "", 1);// 322
            AddComplexComponent((BaseAddon)this, 6077, 5, -9, 0, 2729, -1, "", 1);// 323
            AddComplexComponent((BaseAddon)this, 6077, 4, -8, 0, 2729, -1, "", 1);// 324
            AddComplexComponent((BaseAddon)this, 6077, 4, -9, 0, 2729, -1, "", 1);// 325
            AddComplexComponent((BaseAddon)this, 6077, 3, -8, 0, 2729, -1, "", 1);// 326
            AddComplexComponent((BaseAddon)this, 6077, 3, -9, 0, 2729, -1, "", 1);// 327
            AddComplexComponent((BaseAddon)this, 6077, 2, -8, 0, 2729, -1, "", 1);// 328
            AddComplexComponent((BaseAddon)this, 6077, 2, -9, 0, 2729, -1, "", 1);// 329
            AddComplexComponent((BaseAddon)this, 6077, 9, -6, 0, 2729, -1, "", 1);// 330
            AddComplexComponent((BaseAddon)this, 6077, 9, -7, 0, 2729, -1, "", 1);// 331
            AddComplexComponent((BaseAddon)this, 6077, 8, -6, 0, 2729, -1, "", 1);// 332
            AddComplexComponent((BaseAddon)this, 6077, 8, -7, 0, 2729, -1, "", 1);// 333
            AddComplexComponent((BaseAddon)this, 6077, 7, -6, 0, 2729, -1, "", 1);// 334
            AddComplexComponent((BaseAddon)this, 6077, 7, -7, 0, 2729, -1, "", 1);// 335
            AddComplexComponent((BaseAddon)this, 6077, 6, -6, 0, 2729, -1, "", 1);// 336
            AddComplexComponent((BaseAddon)this, 6077, 6, -7, 0, 2729, -1, "", 1);// 337
            AddComplexComponent((BaseAddon)this, 6077, 5, -6, 0, 2729, -1, "", 1);// 338
            AddComplexComponent((BaseAddon)this, 6077, 5, -7, 0, 2729, -1, "", 1);// 339
            AddComplexComponent((BaseAddon)this, 6077, 4, -6, 0, 2729, -1, "", 1);// 340
            AddComplexComponent((BaseAddon)this, 6077, 4, -7, 0, 2729, -1, "", 1);// 341
            AddComplexComponent((BaseAddon)this, 6077, 3, -6, 0, 2729, -1, "", 1);// 342
            AddComplexComponent((BaseAddon)this, 6077, 3, -7, 0, 2729, -1, "", 1);// 343
            AddComplexComponent((BaseAddon)this, 6077, 2, -6, 0, 2729, -1, "", 1);// 344
            AddComplexComponent((BaseAddon)this, 6077, 2, -7, 0, 2729, -1, "", 1);// 345
            AddComplexComponent((BaseAddon)this, 6077, 1, -9, 0, 2729, -1, "", 1);// 346
            AddComplexComponent((BaseAddon)this, 6077, 0, -9, 0, 2729, -1, "", 1);// 347
            AddComplexComponent((BaseAddon)this, 6077, -1, -9, 0, 2729, -1, "", 1);// 348
            AddComplexComponent((BaseAddon)this, 6077, -2, -9, 0, 2729, -1, "", 1);// 349
            AddComplexComponent((BaseAddon)this, 6077, -3, -9, 0, 2729, -1, "", 1);// 350
            AddComplexComponent((BaseAddon)this, 6077, -4, -9, 0, 2729, -1, "", 1);// 351
            AddComplexComponent((BaseAddon)this, 6077, -5, -9, 0, 2729, -1, "", 1);// 352
            AddComplexComponent((BaseAddon)this, 6077, -6, -9, 0, 2729, -1, "", 1);// 353
            AddComplexComponent((BaseAddon)this, 17754, -8, 5, 0, 2729, -1, "", 1);// 354
            AddComplexComponent((BaseAddon)this, 17754, -9, 5, 0, 2729, -1, "", 1);// 355
            AddComplexComponent((BaseAddon)this, 17760, -10, 5, 0, 2729, -1, "", 1);// 356
            AddComplexComponent((BaseAddon)this, 17758, -11, 5, 0, 2729, -1, "", 1);// 357
            AddComplexComponent((BaseAddon)this, 17754, -12, 5, 0, 2729, -1, "", 1);// 358
            AddComplexComponent((BaseAddon)this, 17756, -12, 4, 0, 2729, -1, "", 1);// 359
            AddComplexComponent((BaseAddon)this, 17724, -11, 4, 0, 2729, -1, "", 1);// 360
            AddComplexComponent((BaseAddon)this, 17757, -10, 4, 0, 2729, -1, "", 1);// 361
            AddComplexComponent((BaseAddon)this, 17729, -9, 4, 0, 2729, -1, "", 1);// 362
            AddComplexComponent((BaseAddon)this, 17782, -13, -3, 0, 1152, -1, "", 1);// 363
            AddComplexComponent((BaseAddon)this, 17782, -13, -2, 0, 1152, -1, "", 1);// 364
            AddComplexComponent((BaseAddon)this, 17782, -13, 0, 0, 1152, -1, "", 1);// 365
            AddComplexComponent((BaseAddon)this, 17783, -13, 1, 0, 1152, -1, "", 1);// 366
            AddComplexComponent((BaseAddon)this, 17783, -13, -1, 0, 1152, -1, "", 1);// 367
            AddComplexComponent((BaseAddon)this, 17783, -13, -4, 0, 1152, -1, "", 1);// 368
            AddComplexComponent((BaseAddon)this, 17781, -13, 2, 0, 1152, -1, "", 1);// 369
            AddComplexComponent((BaseAddon)this, 17781, -13, -5, 0, 1152, -1, "", 1);// 370
            AddComplexComponent((BaseAddon)this, 17734, -8, 4, 0, 2729, -1, "", 1);// 371
            AddComplexComponent((BaseAddon)this, 19122, -9, -1, 0, 1152, -1, "", 1);// 372
            AddComplexComponent((BaseAddon)this, 22300, -11, 7, 0, 1152, -1, "", 1);// 387
            AddComplexComponent((BaseAddon)this, 6077, -8, 8, 0, 2729, -1, "", 1);// 388
            AddComplexComponent((BaseAddon)this, 6077, -8, 7, 0, 2729, -1, "", 1);// 389
            AddComplexComponent((BaseAddon)this, 6077, -8, 6, 0, 2729, -1, "", 1);// 390
            AddComplexComponent((BaseAddon)this, 6077, -8, 5, 0, 2729, -1, "", 1);// 391
            AddComplexComponent((BaseAddon)this, 6077, -8, 4, 0, 2729, -1, "", 1);// 392
            AddComplexComponent((BaseAddon)this, 6077, -8, 3, 0, 2729, -1, "", 1);// 393
            AddComplexComponent((BaseAddon)this, 6077, -9, 8, 0, 2729, -1, "", 1);// 394
            AddComplexComponent((BaseAddon)this, 6077, -9, 7, 0, 2729, -1, "", 1);// 395
            AddComplexComponent((BaseAddon)this, 6077, -9, 6, 0, 2729, -1, "", 1);// 396
            AddComplexComponent((BaseAddon)this, 6077, -9, 5, 0, 2729, -1, "", 1);// 397
            AddComplexComponent((BaseAddon)this, 6077, -9, 4, 0, 2729, -1, "", 1);// 398
            AddComplexComponent((BaseAddon)this, 6077, -9, 3, 0, 2729, -1, "", 1);// 399
            AddComplexComponent((BaseAddon)this, 6077, -10, 8, 0, 2729, -1, "", 1);// 400
            AddComplexComponent((BaseAddon)this, 6077, -10, 7, 0, 2729, -1, "", 1);// 401
            AddComplexComponent((BaseAddon)this, 6077, -10, 6, 0, 2729, -1, "", 1);// 402
            AddComplexComponent((BaseAddon)this, 6077, -10, 5, 0, 2729, -1, "", 1);// 403
            AddComplexComponent((BaseAddon)this, 6077, -10, 4, 0, 2729, -1, "", 1);// 404
            AddComplexComponent((BaseAddon)this, 6077, -10, 3, 0, 2729, -1, "", 1);// 405
            AddComplexComponent((BaseAddon)this, 6077, -11, 8, 0, 2729, -1, "", 1);// 406
            AddComplexComponent((BaseAddon)this, 6077, -11, 7, 0, 2729, -1, "", 1);// 407
            AddComplexComponent((BaseAddon)this, 6077, -11, 6, 0, 2729, -1, "", 1);// 408
            AddComplexComponent((BaseAddon)this, 6077, -11, 5, 0, 2729, -1, "", 1);// 409
            AddComplexComponent((BaseAddon)this, 6077, -11, 4, 0, 2729, -1, "", 1);// 410
            AddComplexComponent((BaseAddon)this, 6077, -11, 3, 0, 2729, -1, "", 1);// 411
            AddComplexComponent((BaseAddon)this, 6077, -7, 2, 0, 2729, -1, "", 1);// 412
            AddComplexComponent((BaseAddon)this, 6077, -7, 1, 0, 2729, -1, "", 1);// 413
            AddComplexComponent((BaseAddon)this, 6077, -7, 0, 0, 2729, -1, "", 1);// 414
            AddComplexComponent((BaseAddon)this, 6077, -7, -1, 0, 2729, -1, "", 1);// 415
            AddComplexComponent((BaseAddon)this, 6077, -7, -2, 0, 2729, -1, "", 1);// 416
            AddComplexComponent((BaseAddon)this, 6077, -8, 2, 0, 2729, -1, "", 1);// 417
            AddComplexComponent((BaseAddon)this, 6077, -8, 1, 0, 2729, -1, "", 1);// 418
            AddComplexComponent((BaseAddon)this, 6077, -8, 0, 0, 2729, -1, "", 1);// 419
            AddComplexComponent((BaseAddon)this, 6077, -8, -1, 0, 2729, -1, "", 1);// 420
            AddComplexComponent((BaseAddon)this, 6077, -8, -2, 0, 2729, -1, "", 1);// 421
            AddComplexComponent((BaseAddon)this, 6077, -9, 2, 0, 2729, -1, "", 1);// 422
            AddComplexComponent((BaseAddon)this, 6077, -9, 1, 0, 2729, -1, "", 1);// 423
            AddComplexComponent((BaseAddon)this, 6077, -9, 0, 0, 2729, -1, "", 1);// 424
            AddComplexComponent((BaseAddon)this, 6077, -9, -1, 0, 2729, -1, "", 1);// 425
            AddComplexComponent((BaseAddon)this, 6077, -9, -2, 0, 2729, -1, "", 1);// 426
            AddComplexComponent((BaseAddon)this, 6077, -10, 2, 0, 2729, -1, "", 1);// 427
            AddComplexComponent((BaseAddon)this, 6077, -10, 1, 0, 2729, -1, "", 1);// 428
            AddComplexComponent((BaseAddon)this, 6077, -10, 0, 0, 2729, -1, "", 1);// 429
            AddComplexComponent((BaseAddon)this, 6077, -10, -1, 0, 2729, -1, "", 1);// 430
            AddComplexComponent((BaseAddon)this, 6077, -10, -2, 0, 2729, -1, "", 1);// 431
            AddComplexComponent((BaseAddon)this, 6077, -11, 2, 0, 2729, -1, "", 1);// 432
            AddComplexComponent((BaseAddon)this, 6077, -11, 1, 0, 2729, -1, "", 1);// 433
            AddComplexComponent((BaseAddon)this, 6077, -11, 0, 0, 2729, -1, "", 1);// 434
            AddComplexComponent((BaseAddon)this, 6077, -11, -1, 0, 2729, -1, "", 1);// 435
            AddComplexComponent((BaseAddon)this, 6077, -11, -2, 0, 2729, -1, "", 1);// 436
            AddComplexComponent((BaseAddon)this, 6077, -12, 2, 0, 2729, -1, "", 1);// 437
            AddComplexComponent((BaseAddon)this, 6077, -12, 1, 0, 2729, -1, "", 1);// 438
            AddComplexComponent((BaseAddon)this, 6077, -12, 0, 0, 2729, -1, "", 1);// 439
            AddComplexComponent((BaseAddon)this, 6077, -12, -1, 0, 2729, -1, "", 1);// 440
            AddComplexComponent((BaseAddon)this, 6077, -12, -2, 0, 2729, -1, "", 1);// 441
            AddComplexComponent((BaseAddon)this, 6077, -7, -3, 0, 2729, -1, "", 1);// 442
            AddComplexComponent((BaseAddon)this, 6077, -7, -4, 0, 2729, -1, "", 1);// 443
            AddComplexComponent((BaseAddon)this, 6077, -7, -5, 0, 2729, -1, "", 1);// 444
            AddComplexComponent((BaseAddon)this, 6077, -8, -3, 0, 2729, -1, "", 1);// 445
            AddComplexComponent((BaseAddon)this, 6077, -8, -4, 0, 2729, -1, "", 1);// 446
            AddComplexComponent((BaseAddon)this, 6077, -8, -5, 0, 2729, -1, "", 1);// 447
            AddComplexComponent((BaseAddon)this, 6077, -9, -3, 0, 2729, -1, "", 1);// 448
            AddComplexComponent((BaseAddon)this, 6077, -9, -4, 0, 2729, -1, "", 1);// 449
            AddComplexComponent((BaseAddon)this, 6077, -9, -5, 0, 2729, -1, "", 1);// 450
            AddComplexComponent((BaseAddon)this, 6077, -10, -3, 0, 2729, -1, "", 1);// 451
            AddComplexComponent((BaseAddon)this, 6077, -10, -4, 0, 2729, -1, "", 1);// 452
            AddComplexComponent((BaseAddon)this, 6077, -10, -5, 0, 2729, -1, "", 1);// 453
            AddComplexComponent((BaseAddon)this, 6077, -11, -3, 0, 2729, -1, "", 1);// 454
            AddComplexComponent((BaseAddon)this, 6077, -11, -4, 0, 2729, -1, "", 1);// 455
            AddComplexComponent((BaseAddon)this, 6077, -11, -5, 0, 2729, -1, "", 1);// 456
            AddComplexComponent((BaseAddon)this, 6077, -12, -3, 0, 2729, -1, "", 1);// 457
            AddComplexComponent((BaseAddon)this, 6077, -12, -4, 0, 2729, -1, "", 1);// 458
            AddComplexComponent((BaseAddon)this, 6077, -12, -5, 0, 2729, -1, "", 1);// 459
            AddComplexComponent((BaseAddon)this, 6077, -13, -5, 0, 2729, -1, "", 1);// 460
            AddComplexComponent((BaseAddon)this, 6077, -13, -4, 0, 2729, -1, "", 1);// 461
            AddComplexComponent((BaseAddon)this, 6077, -13, -3, 0, 2729, -1, "", 1);// 462
            AddComplexComponent((BaseAddon)this, 6077, -13, -2, 0, 2729, -1, "", 1);// 463
            AddComplexComponent((BaseAddon)this, 6077, -13, -1, 0, 2729, -1, "", 1);// 464
            AddComplexComponent((BaseAddon)this, 6077, -13, 0, 0, 2729, -1, "", 1);// 465
            AddComplexComponent((BaseAddon)this, 6077, -13, 1, 0, 2729, -1, "", 1);// 466
            AddComplexComponent((BaseAddon)this, 6077, -13, 2, 0, 2729, -1, "", 1);// 467
            AddComplexComponent((BaseAddon)this, 6077, -13, 3, 0, 2729, -1, "", 1);// 468
            AddComplexComponent((BaseAddon)this, 6077, -12, 3, 0, 2729, -1, "", 1);// 469
            AddComplexComponent((BaseAddon)this, 6077, -12, 4, 0, 2729, -1, "", 1);// 470
            AddComplexComponent((BaseAddon)this, 6077, -12, 5, 0, 2729, -1, "", 1);// 471
            AddComplexComponent((BaseAddon)this, 6077, -12, 6, 0, 2729, -1, "", 1);// 472
            AddComplexComponent((BaseAddon)this, 6077, -12, 7, 0, 2729, -1, "", 1);// 473
            AddComplexComponent((BaseAddon)this, 6077, -12, 8, 0, 2729, -1, "", 1);// 474
            AddComplexComponent((BaseAddon)this, 17306, -8, 9, 0, 2729, -1, "", 1);// 475
            AddComplexComponent((BaseAddon)this, 17306, -9, 9, 0, 2729, -1, "", 1);// 476
            AddComplexComponent((BaseAddon)this, 17306, -10, 9, 0, 2729, -1, "", 1);// 477
            AddComplexComponent((BaseAddon)this, 17306, -11, 9, 0, 2729, -1, "", 1);// 478
            AddComplexComponent((BaseAddon)this, 17306, -12, 9, 0, 2729, -1, "", 1);// 479
            AddComplexComponent((BaseAddon)this, 17783, -13, -6, 0, 1152, -1, "", 1);// 480
            AddComplexComponent((BaseAddon)this, 17783, -13, -7, 0, 1152, -1, "", 1);// 481
            AddComplexComponent((BaseAddon)this, 17781, -13, -8, 0, 1152, -1, "", 1);// 482
            AddComplexComponent((BaseAddon)this, 17780, -9, -9, 0, 1152, -1, "", 1);// 483
            AddComplexComponent((BaseAddon)this, 17780, -7, -9, 0, 1152, -1, "", 1);// 484
            AddComplexComponent((BaseAddon)this, 17779, -11, -9, 0, 1152, -1, "", 1);// 485
            AddComplexComponent((BaseAddon)this, 17779, -12, -9, 0, 1152, -1, "", 1);// 486
            AddComplexComponent((BaseAddon)this, 17779, -8, -9, 0, 1152, -1, "", 1);// 487
            AddComplexComponent((BaseAddon)this, 17778, -10, -9, 0, 1152, -1, "", 1);// 488
            AddComplexComponent((BaseAddon)this, 22300, -12, -7, 0, 1152, -1, "", 1);// 493
            AddComplexComponent((BaseAddon)this, 6077, -7, -6, 0, 2729, -1, "", 1);// 494
            AddComplexComponent((BaseAddon)this, 6077, -7, -7, 0, 2729, -1, "", 1);// 495
            AddComplexComponent((BaseAddon)this, 6077, -7, -8, 0, 2729, -1, "", 1);// 496
            AddComplexComponent((BaseAddon)this, 6077, -8, -6, 0, 2729, -1, "", 1);// 497
            AddComplexComponent((BaseAddon)this, 6077, -8, -7, 0, 2729, -1, "", 1);// 498
            AddComplexComponent((BaseAddon)this, 6077, -8, -8, 0, 2729, -1, "", 1);// 499
            AddComplexComponent((BaseAddon)this, 6077, -9, -6, 0, 2729, -1, "", 1);// 500
            AddComplexComponent((BaseAddon)this, 6077, -9, -7, 0, 2729, -1, "", 1);// 501
            AddComplexComponent((BaseAddon)this, 6077, -9, -8, 0, 2729, -1, "", 1);// 502
            AddComplexComponent((BaseAddon)this, 6077, -10, -6, 0, 2729, -1, "", 1);// 503
            AddComplexComponent((BaseAddon)this, 6077, -10, -7, 0, 2729, -1, "", 1);// 504
            AddComplexComponent((BaseAddon)this, 6077, -10, -8, 0, 2729, -1, "", 1);// 505
            AddComplexComponent((BaseAddon)this, 6077, -11, -6, 0, 2729, -1, "", 1);// 506
            AddComplexComponent((BaseAddon)this, 6077, -11, -7, 0, 2729, -1, "", 1);// 507
            AddComplexComponent((BaseAddon)this, 6077, -11, -8, 0, 2729, -1, "", 1);// 508
            AddComplexComponent((BaseAddon)this, 6077, -12, -6, 0, 2729, -1, "", 1);// 509
            AddComplexComponent((BaseAddon)this, 6077, -12, -7, 0, 2729, -1, "", 1);// 510
            AddComplexComponent((BaseAddon)this, 6077, -12, -8, 0, 2729, -1, "", 1);// 511
            AddComplexComponent((BaseAddon)this, 6077, -7, -9, 0, 2729, -1, "", 1);// 512
            AddComplexComponent((BaseAddon)this, 6077, -8, -9, 0, 2729, -1, "", 1);// 513
            AddComplexComponent((BaseAddon)this, 6077, -9, -9, 0, 2729, -1, "", 1);// 514
            AddComplexComponent((BaseAddon)this, 6077, -10, -9, 0, 2729, -1, "", 1);// 515
            AddComplexComponent((BaseAddon)this, 6077, -11, -9, 0, 2729, -1, "", 1);// 516
            AddComplexComponent((BaseAddon)this, 6077, -12, -9, 0, 2729, -1, "", 1);// 517
            AddComplexComponent((BaseAddon)this, 6077, -13, -9, 0, 2729, -1, "", 1);// 518
            AddComplexComponent((BaseAddon)this, 6077, -13, -8, 0, 2729, -1, "", 1);// 519
            AddComplexComponent((BaseAddon)this, 6077, -13, -7, 0, 2729, -1, "", 1);// 520
            AddComplexComponent((BaseAddon)this, 6077, -13, -6, 0, 2729, -1, "", 1);// 521
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
