namespace Server.Items
{
    public class BlackthornStep8 : BlackthornBaseAddon
    {
        public static BlackthornStep8 InstanceTram { get; set; }
        public static BlackthornStep8 InstanceFel { get; set; }

        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
              {8444, 9, -8, 0}, {4947, 8, -8, 0}, {4954, 8, -7, 0}// 13	110	111	
			, {4955, 8, -6, 0}, {4952, 8, -9, 0}, {4301, -8, -8, 0}// 112	119	153	
			, {9619, 6, 0, 0}, {9775, -8, -1, 0}, {8445, -4, -2, 0}// 289	290	291	
			, {4952, 7, -5, 0}, {4955, 7, -4, 0}, {4962, 6, -3, 0}// 295	296	297	
			, {4962, 6, -2, 0}, {4962, 5, -1, 0}, {4949, 4, 0, 0}// 298	299	300	
			, {4943, 5, 1, 0}, {4957, 5, 2, 0}, {4957, 4, 3, 0}// 301	302	303	
			, {4944, 3, 3, 0}, {4960, 1, 2, 0}, {4963, 0, 2, 0}// 304	305	306	
			, {4967, -1, 2, 0}, {4967, -2, 2, 0}, {4963, -3, 2, 0}// 307	308	309	
			, {4960, -4, 2, 0}, {4961, -5, 2, 0}, {4963, -5, 1, 0}// 310	311	312	
			, {4965, -5, 0, 5}, {4960, -5, 0, 0}, {4967, -5, -1, 0}// 313	314	315	
			, {4963, -5, -2, 0}, {4960, -5, -3, 0}, {4967, -5, -4, 0}// 316	317	318	
			, {4970, -5, -5, 0}, {4964, -5, -5, 2}, {4962, -5, -6, 0}// 319	320	321	
			, {4969, -5, -7, 3}, {4964, -5, -7, 0}, {4960, -5, -8, 0}// 322	323	324	
			, {4964, -5, -8, 5}, {4967, -6, -9, 0}, {4973, -5, -9, 0}// 325	335	336	
			, {4300, -9, -7, 0}, {4299, -10, -6, 0}, {4298, -11, -5, 0}// 347	348	349	
		};

        [Constructable]
        public BlackthornStep8()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 22000, 13, 9, 0, 2548, -1, "", 1);// 1
            AddComplexComponent(this, 22000, 13, 8, 0, 2548, -1, "", 1);// 2
            AddComplexComponent(this, 22000, 12, 9, 0, 2548, -1, "", 1);// 3
            AddComplexComponent(this, 22000, 12, 8, 0, 2548, -1, "", 1);// 4
            AddComplexComponent(this, 22000, 11, 9, 0, 2548, -1, "", 1);// 5
            AddComplexComponent(this, 22000, 11, 8, 0, 2548, -1, "", 1);// 6
            AddComplexComponent(this, 22000, 10, 9, 0, 2548, -1, "", 1);// 7
            AddComplexComponent(this, 22000, 10, 8, 0, 2548, -1, "", 1);// 8
            AddComplexComponent(this, 22000, 9, 9, 0, 2548, -1, "", 1);// 9
            AddComplexComponent(this, 22000, 9, 8, 0, 2548, -1, "", 1);// 10
            AddComplexComponent(this, 22000, 8, 9, 0, 2548, -1, "", 1);// 11
            AddComplexComponent(this, 22000, 8, 8, 0, 2548, -1, "", 1);// 12
            AddComplexComponent(this, 22000, 13, 2, 0, 2548, -1, "", 1);// 14
            AddComplexComponent(this, 22000, 13, 1, 0, 2548, -1, "", 1);// 15
            AddComplexComponent(this, 22000, 13, 0, 0, 2548, -1, "", 1);// 16
            AddComplexComponent(this, 22000, 13, -1, 0, 2548, -1, "", 1);// 17
            AddComplexComponent(this, 22000, 13, -2, 0, 2548, -1, "", 1);// 18
            AddComplexComponent(this, 22000, 13, -3, 0, 2548, -1, "", 1);// 19
            AddComplexComponent(this, 22000, 13, -4, 0, 2548, -1, "", 1);// 20
            AddComplexComponent(this, 22000, 13, -5, 0, 2548, -1, "", 1);// 21
            AddComplexComponent(this, 22000, 13, -6, 0, 2548, -1, "", 1);// 22
            AddComplexComponent(this, 22000, 13, -7, 0, 2548, -1, "", 1);// 23
            AddComplexComponent(this, 22000, 13, -8, 0, 2548, -1, "", 1);// 24
            AddComplexComponent(this, 22000, 12, 2, 0, 2548, -1, "", 1);// 25
            AddComplexComponent(this, 22000, 12, 1, 0, 2548, -1, "", 1);// 26
            AddComplexComponent(this, 22000, 12, 0, 0, 2548, -1, "", 1);// 27
            AddComplexComponent(this, 22000, 12, -1, 0, 2548, -1, "", 1);// 28
            AddComplexComponent(this, 22000, 12, -2, 0, 2548, -1, "", 1);// 29
            AddComplexComponent(this, 22000, 12, -3, 0, 2548, -1, "", 1);// 30
            AddComplexComponent(this, 22000, 12, -4, 0, 2548, -1, "", 1);// 31
            AddComplexComponent(this, 22000, 12, -5, 0, 2548, -1, "", 1);// 32
            AddComplexComponent(this, 22000, 12, -6, 0, 2548, -1, "", 1);// 33
            AddComplexComponent(this, 22000, 12, -7, 0, 2548, -1, "", 1);// 34
            AddComplexComponent(this, 22000, 12, -8, 0, 2548, -1, "", 1);// 35
            AddComplexComponent(this, 22000, 11, 2, 0, 2548, -1, "", 1);// 36
            AddComplexComponent(this, 22000, 11, 1, 0, 2548, -1, "", 1);// 37
            AddComplexComponent(this, 22000, 11, 0, 0, 2548, -1, "", 1);// 38
            AddComplexComponent(this, 22000, 11, -1, 0, 2548, -1, "", 1);// 39
            AddComplexComponent(this, 22000, 11, -2, 0, 2548, -1, "", 1);// 40
            AddComplexComponent(this, 22000, 11, -3, 0, 2548, -1, "", 1);// 41
            AddComplexComponent(this, 22000, 11, -4, 0, 2548, -1, "", 1);// 42
            AddComplexComponent(this, 22000, 11, -5, 0, 2548, -1, "", 1);// 43
            AddComplexComponent(this, 22000, 11, -6, 0, 2548, -1, "", 1);// 44
            AddComplexComponent(this, 22000, 11, -7, 0, 2548, -1, "", 1);// 45
            AddComplexComponent(this, 22000, 11, -8, 0, 2548, -1, "", 1);// 46
            AddComplexComponent(this, 22000, 10, 2, 0, 2548, -1, "", 1);// 47
            AddComplexComponent(this, 22000, 10, 1, 0, 2548, -1, "", 1);// 48
            AddComplexComponent(this, 22000, 10, 0, 0, 2548, -1, "", 1);// 49
            AddComplexComponent(this, 22000, 10, -1, 0, 2548, -1, "", 1);// 50
            AddComplexComponent(this, 22000, 10, -2, 0, 2548, -1, "", 1);// 51
            AddComplexComponent(this, 22000, 10, -3, 0, 2548, -1, "", 1);// 52
            AddComplexComponent(this, 22000, 10, -4, 0, 2548, -1, "", 1);// 53
            AddComplexComponent(this, 22000, 10, -5, 0, 2548, -1, "", 1);// 54
            AddComplexComponent(this, 22000, 10, -6, 0, 2548, -1, "", 1);// 55
            AddComplexComponent(this, 22000, 10, -7, 0, 2548, -1, "", 1);// 56
            AddComplexComponent(this, 22000, 10, -8, 0, 2548, -1, "", 1);// 57
            AddComplexComponent(this, 22000, 9, 2, 0, 2548, -1, "", 1);// 58
            AddComplexComponent(this, 22000, 9, 1, 0, 2548, -1, "", 1);// 59
            AddComplexComponent(this, 22000, 9, 0, 0, 2548, -1, "", 1);// 60
            AddComplexComponent(this, 22000, 9, -1, 0, 2548, -1, "", 1);// 61
            AddComplexComponent(this, 22000, 9, -2, 0, 2548, -1, "", 1);// 62
            AddComplexComponent(this, 22000, 9, -3, 0, 2548, -1, "", 1);// 63
            AddComplexComponent(this, 22000, 9, -4, 0, 2548, -1, "", 1);// 64
            AddComplexComponent(this, 22000, 9, -5, 0, 2548, -1, "", 1);// 65
            AddComplexComponent(this, 22000, 9, -6, 0, 2548, -1, "", 1);// 66
            AddComplexComponent(this, 22000, 9, -7, 0, 2548, -1, "", 1);// 67
            AddComplexComponent(this, 22000, 9, -8, 0, 2548, -1, "", 1);// 68
            AddComplexComponent(this, 22000, 8, 2, 0, 2548, -1, "", 1);// 69
            AddComplexComponent(this, 22000, 8, 1, 0, 2548, -1, "", 1);// 70
            AddComplexComponent(this, 22000, 8, 0, 0, 2548, -1, "", 1);// 71
            AddComplexComponent(this, 22000, 8, -1, 0, 2548, -1, "", 1);// 72
            AddComplexComponent(this, 22000, 8, -2, 0, 2548, -1, "", 1);// 73
            AddComplexComponent(this, 22000, 8, -3, 0, 2548, -1, "", 1);// 74
            AddComplexComponent(this, 22000, 8, -4, 0, 2548, -1, "", 1);// 75
            AddComplexComponent(this, 22000, 8, -5, 0, 2548, -1, "", 1);// 76
            AddComplexComponent(this, 22000, 8, -6, 0, 2548, -1, "", 1);// 77
            AddComplexComponent(this, 22000, 8, -7, 0, 2548, -1, "", 1);// 78
            AddComplexComponent(this, 22000, 8, -8, 0, 2548, -1, "", 1);// 79
            AddComplexComponent(this, 22000, 13, 7, 0, 2548, -1, "", 1);// 80
            AddComplexComponent(this, 22000, 13, 6, 0, 2548, -1, "", 1);// 81
            AddComplexComponent(this, 22000, 13, 5, 0, 2548, -1, "", 1);// 82
            AddComplexComponent(this, 22000, 13, 4, 0, 2548, -1, "", 1);// 83
            AddComplexComponent(this, 22000, 13, 3, 0, 2548, -1, "", 1);// 84
            AddComplexComponent(this, 22000, 12, 7, 0, 2548, -1, "", 1);// 85
            AddComplexComponent(this, 22000, 12, 6, 0, 2548, -1, "", 1);// 86
            AddComplexComponent(this, 22000, 12, 5, 0, 2548, -1, "", 1);// 87
            AddComplexComponent(this, 22000, 12, 4, 0, 2548, -1, "", 1);// 88
            AddComplexComponent(this, 22000, 12, 3, 0, 2548, -1, "", 1);// 89
            AddComplexComponent(this, 22000, 11, 7, 0, 2548, -1, "", 1);// 90
            AddComplexComponent(this, 22000, 11, 6, 0, 2548, -1, "", 1);// 91
            AddComplexComponent(this, 22000, 11, 5, 0, 2548, -1, "", 1);// 92
            AddComplexComponent(this, 22000, 11, 4, 0, 2548, -1, "", 1);// 93
            AddComplexComponent(this, 22000, 11, 3, 0, 2548, -1, "", 1);// 94
            AddComplexComponent(this, 22000, 10, 7, 0, 2548, -1, "", 1);// 95
            AddComplexComponent(this, 22000, 10, 6, 0, 2548, -1, "", 1);// 96
            AddComplexComponent(this, 22000, 10, 5, 0, 2548, -1, "", 1);// 97
            AddComplexComponent(this, 22000, 10, 4, 0, 2548, -1, "", 1);// 98
            AddComplexComponent(this, 22000, 10, 3, 0, 2548, -1, "", 1);// 99
            AddComplexComponent(this, 22000, 9, 7, 0, 2548, -1, "", 1);// 100
            AddComplexComponent(this, 22000, 9, 6, 0, 2548, -1, "", 1);// 101
            AddComplexComponent(this, 22000, 9, 5, 0, 2548, -1, "", 1);// 102
            AddComplexComponent(this, 22000, 9, 4, 0, 2548, -1, "", 1);// 103
            AddComplexComponent(this, 22000, 9, 3, 0, 2548, -1, "", 1);// 104
            AddComplexComponent(this, 22000, 8, 7, 0, 2548, -1, "", 1);// 105
            AddComplexComponent(this, 22000, 8, 6, 0, 2548, -1, "", 1);// 106
            AddComplexComponent(this, 22000, 8, 5, 0, 2548, -1, "", 1);// 107
            AddComplexComponent(this, 22000, 8, 4, 0, 2548, -1, "", 1);// 108
            AddComplexComponent(this, 22000, 8, 3, 0, 2548, -1, "", 1);// 109
            AddComplexComponent(this, 22000, 13, -9, 0, 2548, -1, "", 1);// 113
            AddComplexComponent(this, 22000, 12, -9, 0, 2548, -1, "", 1);// 114
            AddComplexComponent(this, 22000, 11, -9, 0, 2548, -1, "", 1);// 115
            AddComplexComponent(this, 22000, 10, -9, 0, 2548, -1, "", 1);// 116
            AddComplexComponent(this, 22000, 9, -9, 0, 2548, -1, "", 1);// 117
            AddComplexComponent(this, 22000, 8, -9, 0, 2548, -1, "", 1);// 118
            AddComplexComponent(this, 14138, -7, 8, 0, 762, -1, "", 1);// 120
            AddComplexComponent(this, 22000, 7, 9, 0, 2548, -1, "", 1);// 121
            AddComplexComponent(this, 22000, 7, 8, 0, 2548, -1, "", 1);// 122
            AddComplexComponent(this, 22000, 6, 9, 0, 2548, -1, "", 1);// 123
            AddComplexComponent(this, 22000, 6, 8, 0, 2548, -1, "", 1);// 124
            AddComplexComponent(this, 22000, 5, 9, 0, 2548, -1, "", 1);// 125
            AddComplexComponent(this, 22000, 5, 8, 0, 2548, -1, "", 1);// 126
            AddComplexComponent(this, 22000, 4, 9, 0, 2548, -1, "", 1);// 127
            AddComplexComponent(this, 22000, 4, 8, 0, 2548, -1, "", 1);// 128
            AddComplexComponent(this, 22000, 3, 9, 0, 2548, -1, "", 1);// 129
            AddComplexComponent(this, 22000, 3, 8, 0, 2548, -1, "", 1);// 130
            AddComplexComponent(this, 22000, 2, 9, 0, 2548, -1, "", 1);// 131
            AddComplexComponent(this, 22000, 2, 8, 0, 2548, -1, "", 1);// 132
            AddComplexComponent(this, 22000, 1, 9, 0, 2548, -1, "", 1);// 133
            AddComplexComponent(this, 22000, 1, 8, 0, 2548, -1, "", 1);// 134
            AddComplexComponent(this, 22000, 0, 9, 0, 2548, -1, "", 1);// 135
            AddComplexComponent(this, 22000, 0, 8, 0, 2548, -1, "", 1);// 136
            AddComplexComponent(this, 22000, -1, 9, 0, 2548, -1, "", 1);// 137
            AddComplexComponent(this, 22000, -1, 8, 0, 2548, -1, "", 1);// 138
            AddComplexComponent(this, 22000, -2, 9, 0, 2548, -1, "", 1);// 139
            AddComplexComponent(this, 22000, -2, 8, 0, 2548, -1, "", 1);// 140
            AddComplexComponent(this, 22000, -3, 9, 0, 2548, -1, "", 1);// 141
            AddComplexComponent(this, 22000, -3, 8, 0, 2548, -1, "", 1);// 142
            AddComplexComponent(this, 22000, -4, 9, 0, 2548, -1, "", 1);// 143
            AddComplexComponent(this, 22000, -4, 8, 0, 2548, -1, "", 1);// 144
            AddComplexComponent(this, 22000, -5, 9, 0, 2548, -1, "", 1);// 145
            AddComplexComponent(this, 22000, -5, 8, 0, 2548, -1, "", 1);// 146
            AddComplexComponent(this, 22000, -6, 9, 0, 2548, -1, "", 1);// 147
            AddComplexComponent(this, 22000, -6, 8, 0, 2548, -1, "", 1);// 148
            AddComplexComponent(this, 22000, -7, 9, 0, 2548, -1, "", 1);// 149
            AddComplexComponent(this, 22000, -7, 8, 0, 2548, -1, "", 1);// 150
            AddComplexComponent(this, 22000, -8, 9, 0, 2548, -1, "", 1);// 151
            AddComplexComponent(this, 22000, -8, 8, 0, 2548, -1, "", 1);// 152
            AddComplexComponent(this, 8136, -3, 6, 0, 2548, -1, "", 1);// 154
            AddComplexComponent(this, 8446, -2, 6, 0, 1920, -1, "", 1);// 155
            AddComplexComponent(this, 22000, -3, 2, 0, 2548, -1, "", 1);// 156
            AddComplexComponent(this, 22000, -1, 2, 0, 2548, -1, "", 1);// 157
            AddComplexComponent(this, 22000, -2, 2, 0, 2548, -1, "", 1);// 158
            AddComplexComponent(this, 22000, 7, 2, 0, 2548, -1, "", 1);// 159
            AddComplexComponent(this, 22000, 7, 1, 0, 2548, -1, "", 1);// 160
            AddComplexComponent(this, 22000, 7, 0, 0, 2548, -1, "", 1);// 161
            AddComplexComponent(this, 22000, 7, -1, 0, 2548, -1, "", 1);// 162
            AddComplexComponent(this, 22000, 7, -2, 0, 2548, -1, "", 1);// 163
            AddComplexComponent(this, 22000, 7, -3, 0, 2548, -1, "", 1);// 164
            AddComplexComponent(this, 22000, 6, 2, 0, 2548, -1, "", 1);// 165
            AddComplexComponent(this, 22000, 6, 1, 0, 2548, -1, "", 1);// 166
            AddComplexComponent(this, 22000, 6, 0, 0, 2548, -1, "", 1);// 167
            AddComplexComponent(this, 22000, 6, -1, 0, 2548, -1, "", 1);// 168
            AddComplexComponent(this, 22000, 6, -2, 0, 2548, -1, "", 1);// 169
            AddComplexComponent(this, 22000, 6, -3, 0, 2548, -1, "", 1);// 170
            AddComplexComponent(this, 22000, 5, 2, 0, 2548, -1, "", 1);// 171
            AddComplexComponent(this, 22000, 5, 1, 0, 2548, -1, "", 1);// 172
            AddComplexComponent(this, 22000, 5, 0, 0, 2548, -1, "", 1);// 173
            AddComplexComponent(this, 22000, 5, -1, 0, 2548, -1, "", 1);// 174
            AddComplexComponent(this, 22000, 7, 7, 0, 2548, -1, "", 1);// 175
            AddComplexComponent(this, 22000, 7, 6, 0, 2548, -1, "", 1);// 176
            AddComplexComponent(this, 22000, 7, 5, 0, 2548, -1, "", 1);// 177
            AddComplexComponent(this, 22000, 7, 4, 0, 2548, -1, "", 1);// 178
            AddComplexComponent(this, 22000, 7, 3, 0, 2548, -1, "", 1);// 179
            AddComplexComponent(this, 22000, 6, 7, 0, 2548, -1, "", 1);// 180
            AddComplexComponent(this, 22000, 6, 6, 0, 2548, -1, "", 1);// 181
            AddComplexComponent(this, 22000, 6, 5, 0, 2548, -1, "", 1);// 182
            AddComplexComponent(this, 22000, 6, 4, 0, 2548, -1, "", 1);// 183
            AddComplexComponent(this, 22000, 6, 3, 0, 2548, -1, "", 1);// 184
            AddComplexComponent(this, 22000, 5, 7, 0, 2548, -1, "", 1);// 185
            AddComplexComponent(this, 22000, 5, 6, 0, 2548, -1, "", 1);// 186
            AddComplexComponent(this, 22000, 5, 5, 0, 2548, -1, "", 1);// 187
            AddComplexComponent(this, 22000, 5, 4, 0, 2548, -1, "", 1);// 188
            AddComplexComponent(this, 22000, 5, 3, 0, 2548, -1, "", 1);// 189
            AddComplexComponent(this, 22000, 4, 7, 0, 2548, -1, "", 1);// 190
            AddComplexComponent(this, 22000, 4, 6, 0, 2548, -1, "", 1);// 191
            AddComplexComponent(this, 22000, 4, 5, 0, 2548, -1, "", 1);// 192
            AddComplexComponent(this, 22000, 4, 4, 0, 2548, -1, "", 1);// 193
            AddComplexComponent(this, 22000, 4, 3, 0, 2548, -1, "", 1);// 194
            AddComplexComponent(this, 22000, 3, 7, 0, 2548, -1, "", 1);// 195
            AddComplexComponent(this, 22000, 3, 6, 0, 2548, -1, "", 1);// 196
            AddComplexComponent(this, 22000, 3, 5, 0, 2548, -1, "", 1);// 197
            AddComplexComponent(this, 22000, 3, 4, 0, 2548, -1, "", 1);// 198
            AddComplexComponent(this, 22000, 3, 3, 0, 2548, -1, "", 1);// 199
            AddComplexComponent(this, 22000, 2, 7, 0, 2548, -1, "", 1);// 200
            AddComplexComponent(this, 22000, 2, 6, 0, 2548, -1, "", 1);// 201
            AddComplexComponent(this, 22000, 2, 5, 0, 2548, -1, "", 1);// 202
            AddComplexComponent(this, 22000, 2, 4, 0, 2548, -1, "", 1);// 203
            AddComplexComponent(this, 22000, 2, 3, 0, 2548, -1, "", 1);// 204
            AddComplexComponent(this, 22000, 1, 7, 0, 2548, -1, "", 1);// 205
            AddComplexComponent(this, 22000, 1, 6, 0, 2548, -1, "", 1);// 206
            AddComplexComponent(this, 22000, 1, 5, 0, 2548, -1, "", 1);// 207
            AddComplexComponent(this, 22000, 1, 4, 0, 2548, -1, "", 1);// 208
            AddComplexComponent(this, 22000, 1, 3, 0, 2548, -1, "", 1);// 209
            AddComplexComponent(this, 22000, 0, 7, 0, 2548, -1, "", 1);// 210
            AddComplexComponent(this, 22000, 0, 6, 0, 2548, -1, "", 1);// 211
            AddComplexComponent(this, 22000, 0, 5, 0, 2548, -1, "", 1);// 212
            AddComplexComponent(this, 22000, 0, 4, 0, 2548, -1, "", 1);// 213
            AddComplexComponent(this, 22000, 0, 3, 0, 2548, -1, "", 1);// 214
            AddComplexComponent(this, 22000, -1, 7, 0, 2548, -1, "", 1);// 215
            AddComplexComponent(this, 22000, -1, 6, 0, 2548, -1, "", 1);// 216
            AddComplexComponent(this, 22000, -1, 5, 0, 2548, -1, "", 1);// 217
            AddComplexComponent(this, 22000, -1, 4, 0, 2548, -1, "", 1);// 218
            AddComplexComponent(this, 22000, -1, 3, 0, 2548, -1, "", 1);// 219
            AddComplexComponent(this, 22000, -2, 7, 0, 2548, -1, "", 1);// 220
            AddComplexComponent(this, 22000, -2, 6, 0, 2548, -1, "", 1);// 221
            AddComplexComponent(this, 22000, -2, 5, 0, 2548, -1, "", 1);// 222
            AddComplexComponent(this, 22000, -2, 4, 0, 2548, -1, "", 1);// 223
            AddComplexComponent(this, 22000, -2, 3, 0, 2548, -1, "", 1);// 224
            AddComplexComponent(this, 22000, -3, 7, 0, 2548, -1, "", 1);// 225
            AddComplexComponent(this, 22000, -3, 6, 0, 2548, -1, "", 1);// 226
            AddComplexComponent(this, 22000, -3, 5, 0, 2548, -1, "", 1);// 227
            AddComplexComponent(this, 22000, -3, 4, 0, 2548, -1, "", 1);// 228
            AddComplexComponent(this, 22000, -3, 3, 0, 2548, -1, "", 1);// 229
            AddComplexComponent(this, 22000, -4, 7, 0, 2548, -1, "", 1);// 230
            AddComplexComponent(this, 22000, -4, 6, 0, 2548, -1, "", 1);// 231
            AddComplexComponent(this, 22000, -4, 5, 0, 2548, -1, "", 1);// 232
            AddComplexComponent(this, 22000, -4, 4, 0, 2548, -1, "", 1);// 233
            AddComplexComponent(this, 22000, -4, 3, 0, 2548, -1, "", 1);// 234
            AddComplexComponent(this, 22000, -5, 7, 0, 2548, -1, "", 1);// 235
            AddComplexComponent(this, 22000, -5, 6, 0, 2548, -1, "", 1);// 236
            AddComplexComponent(this, 22000, -5, 5, 0, 2548, -1, "", 1);// 237
            AddComplexComponent(this, 22000, -5, 4, 0, 2548, -1, "", 1);// 238
            AddComplexComponent(this, 22000, -5, 3, 0, 2548, -1, "", 1);// 239
            AddComplexComponent(this, 22000, -5, 2, 0, 2548, -1, "", 1);// 240
            AddComplexComponent(this, 22000, -6, 7, 0, 2548, -1, "", 1);// 241
            AddComplexComponent(this, 22000, -6, 6, 0, 2548, -1, "", 1);// 242
            AddComplexComponent(this, 22000, -6, 5, 0, 2548, -1, "", 1);// 243
            AddComplexComponent(this, 22000, -6, 4, 0, 2548, -1, "", 1);// 244
            AddComplexComponent(this, 22000, -6, 3, 0, 2548, -1, "", 1);// 245
            AddComplexComponent(this, 22000, -6, 2, 0, 2548, -1, "", 1);// 246
            AddComplexComponent(this, 22000, -7, 7, 0, 2548, -1, "", 1);// 247
            AddComplexComponent(this, 22000, -7, 6, 0, 2548, -1, "", 1);// 248
            AddComplexComponent(this, 22000, -7, 5, 0, 2548, -1, "", 1);// 249
            AddComplexComponent(this, 22000, -7, 4, 0, 2548, -1, "", 1);// 250
            AddComplexComponent(this, 22000, -7, 3, 0, 2548, -1, "", 1);// 251
            AddComplexComponent(this, 22000, -7, 2, 0, 2548, -1, "", 1);// 252
            AddComplexComponent(this, 22000, -8, 7, 0, 2548, -1, "", 1);// 253
            AddComplexComponent(this, 22000, -8, 6, 0, 2548, -1, "", 1);// 254
            AddComplexComponent(this, 22000, -8, 5, 0, 2548, -1, "", 1);// 255
            AddComplexComponent(this, 22000, -8, 4, 0, 2548, -1, "", 1);// 256
            AddComplexComponent(this, 22000, -8, 3, 0, 2548, -1, "", 1);// 257
            AddComplexComponent(this, 22000, -8, 2, 0, 2548, -1, "", 1);// 258
            AddComplexComponent(this, 22000, -6, 1, 0, 2548, -1, "", 1);// 259
            AddComplexComponent(this, 22000, -6, 0, 0, 2548, -1, "", 1);// 260
            AddComplexComponent(this, 22000, -6, -1, 0, 2548, -1, "", 1);// 261
            AddComplexComponent(this, 22000, -6, -2, 0, 2548, -1, "", 1);// 262
            AddComplexComponent(this, 22000, -6, -3, 0, 2548, -1, "", 1);// 263
            AddComplexComponent(this, 22000, -6, -4, 0, 2548, -1, "", 1);// 264
            AddComplexComponent(this, 22000, -6, -5, 0, 2548, -1, "", 1);// 265
            AddComplexComponent(this, 22000, -7, 1, 0, 2548, -1, "", 1);// 266
            AddComplexComponent(this, 22000, -7, 0, 0, 2548, -1, "", 1);// 267
            AddComplexComponent(this, 22000, -7, -1, 0, 2548, -1, "", 1);// 268
            AddComplexComponent(this, 22000, -7, -2, 0, 2548, -1, "", 1);// 269
            AddComplexComponent(this, 22000, -7, -3, 0, 2548, -1, "", 1);// 270
            AddComplexComponent(this, 22000, -7, -4, 0, 2548, -1, "", 1);// 271
            AddComplexComponent(this, 22000, -7, -5, 0, 2548, -1, "", 1);// 272
            AddComplexComponent(this, 22000, -8, 1, 0, 2548, -1, "", 1);// 273
            AddComplexComponent(this, 22000, -8, 0, 0, 2548, -1, "", 1);// 274
            AddComplexComponent(this, 22000, -8, -1, 0, 2548, -1, "", 1);// 275
            AddComplexComponent(this, 22000, -8, -2, 0, 2548, -1, "", 1);// 276
            AddComplexComponent(this, 22000, -8, -3, 0, 2548, -1, "", 1);// 277
            AddComplexComponent(this, 22000, -8, -4, 0, 2548, -1, "", 1);// 278
            AddComplexComponent(this, 22000, -8, -5, 0, 2548, -1, "", 1);// 279
            AddComplexComponent(this, 22000, -6, -6, 0, 2548, -1, "", 1);// 280
            AddComplexComponent(this, 22000, -6, -7, 0, 2548, -1, "", 1);// 281
            AddComplexComponent(this, 22000, -6, -8, 0, 2548, -1, "", 1);// 282
            AddComplexComponent(this, 22000, -7, -6, 0, 2548, -1, "", 1);// 283
            AddComplexComponent(this, 22000, -7, -7, 0, 2548, -1, "", 1);// 284
            AddComplexComponent(this, 22000, -7, -8, 0, 2548, -1, "", 1);// 285
            AddComplexComponent(this, 22000, -8, -6, 0, 2548, -1, "", 1);// 286
            AddComplexComponent(this, 22000, -8, -7, 0, 2548, -1, "", 1);// 287
            AddComplexComponent(this, 22000, -8, -8, 0, 2548, -1, "", 1);// 288
            AddComplexComponent(this, 14120, 4, -8, 0, 762, -1, "", 1);// 292
            AddComplexComponent(this, 14120, 6, -8, 0, 762, -1, "", 1);// 293
            AddComplexComponent(this, 14120, 7, -8, 0, 762, -1, "", 1);// 294
            AddComplexComponent(this, 22000, -6, -9, 0, 2548, -1, "", 1);// 326
            AddComplexComponent(this, 22000, -7, -9, 0, 2548, -1, "", 1);// 327
            AddComplexComponent(this, 22000, -8, -9, 0, 2548, -1, "", 1);// 328
            AddComplexComponent(this, 7897, 6, -9, 10, 2548, -1, "", 1);// 329
            AddComplexComponent(this, 14120, 3, -9, 0, 762, -1, "", 1);// 330
            AddComplexComponent(this, 14120, 2, -9, 0, 762, -1, "", 1);// 331
            AddComplexComponent(this, 14120, 1, -9, 0, 762, -1, "", 1);// 332
            AddComplexComponent(this, 14120, 0, -9, 0, 762, -1, "", 1);// 333
            AddComplexComponent(this, 14120, -1, -9, 0, 762, -1, "", 1);// 334
            AddComplexComponent(this, 22000, -9, 9, 0, 2548, -1, "", 1);// 337
            AddComplexComponent(this, 22000, -9, 8, 0, 2548, -1, "", 1);// 338
            AddComplexComponent(this, 22000, -10, 9, 0, 2548, -1, "", 1);// 339
            AddComplexComponent(this, 22000, -10, 8, 0, 2548, -1, "", 1);// 340
            AddComplexComponent(this, 22000, -11, 9, 0, 2548, -1, "", 1);// 341
            AddComplexComponent(this, 22000, -11, 8, 0, 2548, -1, "", 1);// 342
            AddComplexComponent(this, 22000, -12, 9, 0, 2548, -1, "", 1);// 343
            AddComplexComponent(this, 22000, -12, 8, 0, 2548, -1, "", 1);// 344
            AddComplexComponent(this, 22000, -13, 9, 0, 2548, -1, "", 1);// 345
            AddComplexComponent(this, 22000, -13, 8, 0, 2548, -1, "", 1);// 346
            AddComplexComponent(this, 22000, -9, 7, 0, 2548, -1, "", 1);// 350
            AddComplexComponent(this, 22000, -9, 6, 0, 2548, -1, "", 1);// 351
            AddComplexComponent(this, 22000, -9, 5, 0, 2548, -1, "", 1);// 352
            AddComplexComponent(this, 22000, -9, 4, 0, 2548, -1, "", 1);// 353
            AddComplexComponent(this, 22000, -9, 3, 0, 2548, -1, "", 1);// 354
            AddComplexComponent(this, 22000, -9, 2, 0, 2548, -1, "", 1);// 355
            AddComplexComponent(this, 22000, -10, 7, 0, 2548, -1, "", 1);// 356
            AddComplexComponent(this, 22000, -10, 6, 0, 2548, -1, "", 1);// 357
            AddComplexComponent(this, 22000, -10, 5, 0, 2548, -1, "", 1);// 358
            AddComplexComponent(this, 22000, -10, 4, 0, 2548, -1, "", 1);// 359
            AddComplexComponent(this, 22000, -10, 3, 0, 2548, -1, "", 1);// 360
            AddComplexComponent(this, 22000, -10, 2, 0, 2548, -1, "", 1);// 361
            AddComplexComponent(this, 22000, -11, 7, 0, 2548, -1, "", 1);// 362
            AddComplexComponent(this, 22000, -11, 6, 0, 2548, -1, "", 1);// 363
            AddComplexComponent(this, 22000, -11, 5, 0, 2548, -1, "", 1);// 364
            AddComplexComponent(this, 22000, -11, 4, 0, 2548, -1, "", 1);// 365
            AddComplexComponent(this, 22000, -11, 3, 0, 2548, -1, "", 1);// 366
            AddComplexComponent(this, 22000, -11, 2, 0, 2548, -1, "", 1);// 367
            AddComplexComponent(this, 22000, -12, 7, 0, 2548, -1, "", 1);// 368
            AddComplexComponent(this, 22000, -12, 6, 0, 2548, -1, "", 1);// 369
            AddComplexComponent(this, 22000, -12, 5, 0, 2548, -1, "", 1);// 370
            AddComplexComponent(this, 22000, -12, 4, 0, 2548, -1, "", 1);// 371
            AddComplexComponent(this, 22000, -12, 3, 0, 2548, -1, "", 1);// 372
            AddComplexComponent(this, 22000, -12, 2, 0, 2548, -1, "", 1);// 373
            AddComplexComponent(this, 22000, -13, 7, 0, 2548, -1, "", 1);// 374
            AddComplexComponent(this, 22000, -13, 6, 0, 2548, -1, "", 1);// 375
            AddComplexComponent(this, 22000, -13, 5, 0, 2548, -1, "", 1);// 376
            AddComplexComponent(this, 22000, -13, 4, 0, 2548, -1, "", 1);// 377
            AddComplexComponent(this, 22000, -13, 3, 0, 2548, -1, "", 1);// 378
            AddComplexComponent(this, 22000, -13, 2, 0, 2548, -1, "", 1);// 379
            AddComplexComponent(this, 22000, -9, 1, 0, 2548, -1, "", 1);// 380
            AddComplexComponent(this, 22000, -9, 0, 0, 2548, -1, "", 1);// 381
            AddComplexComponent(this, 22000, -9, -1, 0, 2548, -1, "", 1);// 382
            AddComplexComponent(this, 22000, -9, -2, 0, 2548, -1, "", 1);// 383
            AddComplexComponent(this, 22000, -9, -3, 0, 2548, -1, "", 1);// 384
            AddComplexComponent(this, 22000, -9, -4, 0, 2548, -1, "", 1);// 385
            AddComplexComponent(this, 22000, -9, -5, 0, 2548, -1, "", 1);// 386
            AddComplexComponent(this, 22000, -10, 1, 0, 2548, -1, "", 1);// 387
            AddComplexComponent(this, 22000, -10, 0, 0, 2548, -1, "", 1);// 388
            AddComplexComponent(this, 22000, -10, -1, 0, 2548, -1, "", 1);// 389
            AddComplexComponent(this, 22000, -10, -2, 0, 2548, -1, "", 1);// 390
            AddComplexComponent(this, 22000, -10, -3, 0, 2548, -1, "", 1);// 391
            AddComplexComponent(this, 22000, -10, -4, 0, 2548, -1, "", 1);// 392
            AddComplexComponent(this, 22000, -10, -5, 0, 2548, -1, "", 1);// 393
            AddComplexComponent(this, 22000, -11, 1, 0, 2548, -1, "", 1);// 394
            AddComplexComponent(this, 22000, -11, 0, 0, 2548, -1, "", 1);// 395
            AddComplexComponent(this, 22000, -11, -1, 0, 2548, -1, "", 1);// 396
            AddComplexComponent(this, 22000, -11, -2, 0, 2548, -1, "", 1);// 397
            AddComplexComponent(this, 22000, -11, -3, 0, 2548, -1, "", 1);// 398
            AddComplexComponent(this, 22000, -11, -4, 0, 2548, -1, "", 1);// 399
            AddComplexComponent(this, 22000, -11, -5, 0, 2548, -1, "", 1);// 400
            AddComplexComponent(this, 22000, -12, 1, 0, 2548, -1, "", 1);// 401
            AddComplexComponent(this, 22000, -12, 0, 0, 2548, -1, "", 1);// 402
            AddComplexComponent(this, 22000, -12, -1, 0, 2548, -1, "", 1);// 403
            AddComplexComponent(this, 22000, -12, -2, 0, 2548, -1, "", 1);// 404
            AddComplexComponent(this, 22000, -12, -3, 0, 2548, -1, "", 1);// 405
            AddComplexComponent(this, 22000, -12, -4, 0, 2548, -1, "", 1);// 406
            AddComplexComponent(this, 22000, -12, -5, 0, 2548, -1, "", 1);// 407
            AddComplexComponent(this, 22000, -13, 1, 0, 2548, -1, "", 1);// 408
            AddComplexComponent(this, 22000, -13, 0, 0, 2548, -1, "", 1);// 409
            AddComplexComponent(this, 22000, -13, -1, 0, 2548, -1, "", 1);// 410
            AddComplexComponent(this, 22000, -13, -2, 0, 2548, -1, "", 1);// 411
            AddComplexComponent(this, 22000, -13, -3, 0, 2548, -1, "", 1);// 412
            AddComplexComponent(this, 22000, -13, -4, 0, 2548, -1, "", 1);// 413
            AddComplexComponent(this, 22000, -13, -5, 0, 2548, -1, "", 1);// 414
            AddComplexComponent(this, 22000, -9, -6, 0, 2548, -1, "", 1);// 415
            AddComplexComponent(this, 22000, -9, -7, 0, 2548, -1, "", 1);// 416
            AddComplexComponent(this, 22000, -9, -8, 0, 2548, -1, "", 1);// 417
            AddComplexComponent(this, 22000, -10, -6, 0, 2548, -1, "", 1);// 418
            AddComplexComponent(this, 22000, -10, -7, 0, 2548, -1, "", 1);// 419
            AddComplexComponent(this, 22000, -10, -8, 0, 2548, -1, "", 1);// 420
            AddComplexComponent(this, 22000, -11, -6, 0, 2548, -1, "", 1);// 421
            AddComplexComponent(this, 22000, -11, -7, 0, 2548, -1, "", 1);// 422
            AddComplexComponent(this, 22000, -11, -8, 0, 2548, -1, "", 1);// 423
            AddComplexComponent(this, 22000, -12, -6, 0, 2548, -1, "", 1);// 424
            AddComplexComponent(this, 22000, -12, -7, 0, 2548, -1, "", 1);// 425
            AddComplexComponent(this, 22000, -12, -8, 0, 2548, -1, "", 1);// 426
            AddComplexComponent(this, 22000, -13, -6, 0, 2548, -1, "", 1);// 427
            AddComplexComponent(this, 22000, -13, -7, 0, 2548, -1, "", 1);// 428
            AddComplexComponent(this, 22000, -13, -8, 0, 2548, -1, "", 1);// 429
            AddComplexComponent(this, 22000, -9, -9, 0, 2548, -1, "", 1);// 430
            AddComplexComponent(this, 22000, -10, -9, 0, 2548, -1, "", 1);// 431
            AddComplexComponent(this, 22000, -11, -9, 0, 2548, -1, "", 1);// 432
            AddComplexComponent(this, 22000, -12, -9, 0, 2548, -1, "", 1);// 433
            AddComplexComponent(this, 22000, -13, -9, 0, 2548, -1, "", 1);// 434
        }

        public BlackthornStep8(Serial serial)
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
