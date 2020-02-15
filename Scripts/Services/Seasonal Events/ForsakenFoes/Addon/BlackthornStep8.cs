using System;

namespace Server.Items
{
    public class BlackthornStep8 : BlackthornBaseAddon
    {
        public static BlackthornStep8 InstanceTram { get; set; }
        public static BlackthornStep8 InstanceFel { get; set; }

        private static int[,] m_AddOnSimpleComponents = new int[,]
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

            AddComplexComponent((BaseAddon)this, 22000, 13, 9, 0, 2548, -1, "", 1);// 1
            AddComplexComponent((BaseAddon)this, 22000, 13, 8, 0, 2548, -1, "", 1);// 2
            AddComplexComponent((BaseAddon)this, 22000, 12, 9, 0, 2548, -1, "", 1);// 3
            AddComplexComponent((BaseAddon)this, 22000, 12, 8, 0, 2548, -1, "", 1);// 4
            AddComplexComponent((BaseAddon)this, 22000, 11, 9, 0, 2548, -1, "", 1);// 5
            AddComplexComponent((BaseAddon)this, 22000, 11, 8, 0, 2548, -1, "", 1);// 6
            AddComplexComponent((BaseAddon)this, 22000, 10, 9, 0, 2548, -1, "", 1);// 7
            AddComplexComponent((BaseAddon)this, 22000, 10, 8, 0, 2548, -1, "", 1);// 8
            AddComplexComponent((BaseAddon)this, 22000, 9, 9, 0, 2548, -1, "", 1);// 9
            AddComplexComponent((BaseAddon)this, 22000, 9, 8, 0, 2548, -1, "", 1);// 10
            AddComplexComponent((BaseAddon)this, 22000, 8, 9, 0, 2548, -1, "", 1);// 11
            AddComplexComponent((BaseAddon)this, 22000, 8, 8, 0, 2548, -1, "", 1);// 12
            AddComplexComponent((BaseAddon)this, 22000, 13, 2, 0, 2548, -1, "", 1);// 14
            AddComplexComponent((BaseAddon)this, 22000, 13, 1, 0, 2548, -1, "", 1);// 15
            AddComplexComponent((BaseAddon)this, 22000, 13, 0, 0, 2548, -1, "", 1);// 16
            AddComplexComponent((BaseAddon)this, 22000, 13, -1, 0, 2548, -1, "", 1);// 17
            AddComplexComponent((BaseAddon)this, 22000, 13, -2, 0, 2548, -1, "", 1);// 18
            AddComplexComponent((BaseAddon)this, 22000, 13, -3, 0, 2548, -1, "", 1);// 19
            AddComplexComponent((BaseAddon)this, 22000, 13, -4, 0, 2548, -1, "", 1);// 20
            AddComplexComponent((BaseAddon)this, 22000, 13, -5, 0, 2548, -1, "", 1);// 21
            AddComplexComponent((BaseAddon)this, 22000, 13, -6, 0, 2548, -1, "", 1);// 22
            AddComplexComponent((BaseAddon)this, 22000, 13, -7, 0, 2548, -1, "", 1);// 23
            AddComplexComponent((BaseAddon)this, 22000, 13, -8, 0, 2548, -1, "", 1);// 24
            AddComplexComponent((BaseAddon)this, 22000, 12, 2, 0, 2548, -1, "", 1);// 25
            AddComplexComponent((BaseAddon)this, 22000, 12, 1, 0, 2548, -1, "", 1);// 26
            AddComplexComponent((BaseAddon)this, 22000, 12, 0, 0, 2548, -1, "", 1);// 27
            AddComplexComponent((BaseAddon)this, 22000, 12, -1, 0, 2548, -1, "", 1);// 28
            AddComplexComponent((BaseAddon)this, 22000, 12, -2, 0, 2548, -1, "", 1);// 29
            AddComplexComponent((BaseAddon)this, 22000, 12, -3, 0, 2548, -1, "", 1);// 30
            AddComplexComponent((BaseAddon)this, 22000, 12, -4, 0, 2548, -1, "", 1);// 31
            AddComplexComponent((BaseAddon)this, 22000, 12, -5, 0, 2548, -1, "", 1);// 32
            AddComplexComponent((BaseAddon)this, 22000, 12, -6, 0, 2548, -1, "", 1);// 33
            AddComplexComponent((BaseAddon)this, 22000, 12, -7, 0, 2548, -1, "", 1);// 34
            AddComplexComponent((BaseAddon)this, 22000, 12, -8, 0, 2548, -1, "", 1);// 35
            AddComplexComponent((BaseAddon)this, 22000, 11, 2, 0, 2548, -1, "", 1);// 36
            AddComplexComponent((BaseAddon)this, 22000, 11, 1, 0, 2548, -1, "", 1);// 37
            AddComplexComponent((BaseAddon)this, 22000, 11, 0, 0, 2548, -1, "", 1);// 38
            AddComplexComponent((BaseAddon)this, 22000, 11, -1, 0, 2548, -1, "", 1);// 39
            AddComplexComponent((BaseAddon)this, 22000, 11, -2, 0, 2548, -1, "", 1);// 40
            AddComplexComponent((BaseAddon)this, 22000, 11, -3, 0, 2548, -1, "", 1);// 41
            AddComplexComponent((BaseAddon)this, 22000, 11, -4, 0, 2548, -1, "", 1);// 42
            AddComplexComponent((BaseAddon)this, 22000, 11, -5, 0, 2548, -1, "", 1);// 43
            AddComplexComponent((BaseAddon)this, 22000, 11, -6, 0, 2548, -1, "", 1);// 44
            AddComplexComponent((BaseAddon)this, 22000, 11, -7, 0, 2548, -1, "", 1);// 45
            AddComplexComponent((BaseAddon)this, 22000, 11, -8, 0, 2548, -1, "", 1);// 46
            AddComplexComponent((BaseAddon)this, 22000, 10, 2, 0, 2548, -1, "", 1);// 47
            AddComplexComponent((BaseAddon)this, 22000, 10, 1, 0, 2548, -1, "", 1);// 48
            AddComplexComponent((BaseAddon)this, 22000, 10, 0, 0, 2548, -1, "", 1);// 49
            AddComplexComponent((BaseAddon)this, 22000, 10, -1, 0, 2548, -1, "", 1);// 50
            AddComplexComponent((BaseAddon)this, 22000, 10, -2, 0, 2548, -1, "", 1);// 51
            AddComplexComponent((BaseAddon)this, 22000, 10, -3, 0, 2548, -1, "", 1);// 52
            AddComplexComponent((BaseAddon)this, 22000, 10, -4, 0, 2548, -1, "", 1);// 53
            AddComplexComponent((BaseAddon)this, 22000, 10, -5, 0, 2548, -1, "", 1);// 54
            AddComplexComponent((BaseAddon)this, 22000, 10, -6, 0, 2548, -1, "", 1);// 55
            AddComplexComponent((BaseAddon)this, 22000, 10, -7, 0, 2548, -1, "", 1);// 56
            AddComplexComponent((BaseAddon)this, 22000, 10, -8, 0, 2548, -1, "", 1);// 57
            AddComplexComponent((BaseAddon)this, 22000, 9, 2, 0, 2548, -1, "", 1);// 58
            AddComplexComponent((BaseAddon)this, 22000, 9, 1, 0, 2548, -1, "", 1);// 59
            AddComplexComponent((BaseAddon)this, 22000, 9, 0, 0, 2548, -1, "", 1);// 60
            AddComplexComponent((BaseAddon)this, 22000, 9, -1, 0, 2548, -1, "", 1);// 61
            AddComplexComponent((BaseAddon)this, 22000, 9, -2, 0, 2548, -1, "", 1);// 62
            AddComplexComponent((BaseAddon)this, 22000, 9, -3, 0, 2548, -1, "", 1);// 63
            AddComplexComponent((BaseAddon)this, 22000, 9, -4, 0, 2548, -1, "", 1);// 64
            AddComplexComponent((BaseAddon)this, 22000, 9, -5, 0, 2548, -1, "", 1);// 65
            AddComplexComponent((BaseAddon)this, 22000, 9, -6, 0, 2548, -1, "", 1);// 66
            AddComplexComponent((BaseAddon)this, 22000, 9, -7, 0, 2548, -1, "", 1);// 67
            AddComplexComponent((BaseAddon)this, 22000, 9, -8, 0, 2548, -1, "", 1);// 68
            AddComplexComponent((BaseAddon)this, 22000, 8, 2, 0, 2548, -1, "", 1);// 69
            AddComplexComponent((BaseAddon)this, 22000, 8, 1, 0, 2548, -1, "", 1);// 70
            AddComplexComponent((BaseAddon)this, 22000, 8, 0, 0, 2548, -1, "", 1);// 71
            AddComplexComponent((BaseAddon)this, 22000, 8, -1, 0, 2548, -1, "", 1);// 72
            AddComplexComponent((BaseAddon)this, 22000, 8, -2, 0, 2548, -1, "", 1);// 73
            AddComplexComponent((BaseAddon)this, 22000, 8, -3, 0, 2548, -1, "", 1);// 74
            AddComplexComponent((BaseAddon)this, 22000, 8, -4, 0, 2548, -1, "", 1);// 75
            AddComplexComponent((BaseAddon)this, 22000, 8, -5, 0, 2548, -1, "", 1);// 76
            AddComplexComponent((BaseAddon)this, 22000, 8, -6, 0, 2548, -1, "", 1);// 77
            AddComplexComponent((BaseAddon)this, 22000, 8, -7, 0, 2548, -1, "", 1);// 78
            AddComplexComponent((BaseAddon)this, 22000, 8, -8, 0, 2548, -1, "", 1);// 79
            AddComplexComponent((BaseAddon)this, 22000, 13, 7, 0, 2548, -1, "", 1);// 80
            AddComplexComponent((BaseAddon)this, 22000, 13, 6, 0, 2548, -1, "", 1);// 81
            AddComplexComponent((BaseAddon)this, 22000, 13, 5, 0, 2548, -1, "", 1);// 82
            AddComplexComponent((BaseAddon)this, 22000, 13, 4, 0, 2548, -1, "", 1);// 83
            AddComplexComponent((BaseAddon)this, 22000, 13, 3, 0, 2548, -1, "", 1);// 84
            AddComplexComponent((BaseAddon)this, 22000, 12, 7, 0, 2548, -1, "", 1);// 85
            AddComplexComponent((BaseAddon)this, 22000, 12, 6, 0, 2548, -1, "", 1);// 86
            AddComplexComponent((BaseAddon)this, 22000, 12, 5, 0, 2548, -1, "", 1);// 87
            AddComplexComponent((BaseAddon)this, 22000, 12, 4, 0, 2548, -1, "", 1);// 88
            AddComplexComponent((BaseAddon)this, 22000, 12, 3, 0, 2548, -1, "", 1);// 89
            AddComplexComponent((BaseAddon)this, 22000, 11, 7, 0, 2548, -1, "", 1);// 90
            AddComplexComponent((BaseAddon)this, 22000, 11, 6, 0, 2548, -1, "", 1);// 91
            AddComplexComponent((BaseAddon)this, 22000, 11, 5, 0, 2548, -1, "", 1);// 92
            AddComplexComponent((BaseAddon)this, 22000, 11, 4, 0, 2548, -1, "", 1);// 93
            AddComplexComponent((BaseAddon)this, 22000, 11, 3, 0, 2548, -1, "", 1);// 94
            AddComplexComponent((BaseAddon)this, 22000, 10, 7, 0, 2548, -1, "", 1);// 95
            AddComplexComponent((BaseAddon)this, 22000, 10, 6, 0, 2548, -1, "", 1);// 96
            AddComplexComponent((BaseAddon)this, 22000, 10, 5, 0, 2548, -1, "", 1);// 97
            AddComplexComponent((BaseAddon)this, 22000, 10, 4, 0, 2548, -1, "", 1);// 98
            AddComplexComponent((BaseAddon)this, 22000, 10, 3, 0, 2548, -1, "", 1);// 99
            AddComplexComponent((BaseAddon)this, 22000, 9, 7, 0, 2548, -1, "", 1);// 100
            AddComplexComponent((BaseAddon)this, 22000, 9, 6, 0, 2548, -1, "", 1);// 101
            AddComplexComponent((BaseAddon)this, 22000, 9, 5, 0, 2548, -1, "", 1);// 102
            AddComplexComponent((BaseAddon)this, 22000, 9, 4, 0, 2548, -1, "", 1);// 103
            AddComplexComponent((BaseAddon)this, 22000, 9, 3, 0, 2548, -1, "", 1);// 104
            AddComplexComponent((BaseAddon)this, 22000, 8, 7, 0, 2548, -1, "", 1);// 105
            AddComplexComponent((BaseAddon)this, 22000, 8, 6, 0, 2548, -1, "", 1);// 106
            AddComplexComponent((BaseAddon)this, 22000, 8, 5, 0, 2548, -1, "", 1);// 107
            AddComplexComponent((BaseAddon)this, 22000, 8, 4, 0, 2548, -1, "", 1);// 108
            AddComplexComponent((BaseAddon)this, 22000, 8, 3, 0, 2548, -1, "", 1);// 109
            AddComplexComponent((BaseAddon)this, 22000, 13, -9, 0, 2548, -1, "", 1);// 113
            AddComplexComponent((BaseAddon)this, 22000, 12, -9, 0, 2548, -1, "", 1);// 114
            AddComplexComponent((BaseAddon)this, 22000, 11, -9, 0, 2548, -1, "", 1);// 115
            AddComplexComponent((BaseAddon)this, 22000, 10, -9, 0, 2548, -1, "", 1);// 116
            AddComplexComponent((BaseAddon)this, 22000, 9, -9, 0, 2548, -1, "", 1);// 117
            AddComplexComponent((BaseAddon)this, 22000, 8, -9, 0, 2548, -1, "", 1);// 118
            AddComplexComponent((BaseAddon)this, 14138, -7, 8, 0, 762, -1, "", 1);// 120
            AddComplexComponent((BaseAddon)this, 22000, 7, 9, 0, 2548, -1, "", 1);// 121
            AddComplexComponent((BaseAddon)this, 22000, 7, 8, 0, 2548, -1, "", 1);// 122
            AddComplexComponent((BaseAddon)this, 22000, 6, 9, 0, 2548, -1, "", 1);// 123
            AddComplexComponent((BaseAddon)this, 22000, 6, 8, 0, 2548, -1, "", 1);// 124
            AddComplexComponent((BaseAddon)this, 22000, 5, 9, 0, 2548, -1, "", 1);// 125
            AddComplexComponent((BaseAddon)this, 22000, 5, 8, 0, 2548, -1, "", 1);// 126
            AddComplexComponent((BaseAddon)this, 22000, 4, 9, 0, 2548, -1, "", 1);// 127
            AddComplexComponent((BaseAddon)this, 22000, 4, 8, 0, 2548, -1, "", 1);// 128
            AddComplexComponent((BaseAddon)this, 22000, 3, 9, 0, 2548, -1, "", 1);// 129
            AddComplexComponent((BaseAddon)this, 22000, 3, 8, 0, 2548, -1, "", 1);// 130
            AddComplexComponent((BaseAddon)this, 22000, 2, 9, 0, 2548, -1, "", 1);// 131
            AddComplexComponent((BaseAddon)this, 22000, 2, 8, 0, 2548, -1, "", 1);// 132
            AddComplexComponent((BaseAddon)this, 22000, 1, 9, 0, 2548, -1, "", 1);// 133
            AddComplexComponent((BaseAddon)this, 22000, 1, 8, 0, 2548, -1, "", 1);// 134
            AddComplexComponent((BaseAddon)this, 22000, 0, 9, 0, 2548, -1, "", 1);// 135
            AddComplexComponent((BaseAddon)this, 22000, 0, 8, 0, 2548, -1, "", 1);// 136
            AddComplexComponent((BaseAddon)this, 22000, -1, 9, 0, 2548, -1, "", 1);// 137
            AddComplexComponent((BaseAddon)this, 22000, -1, 8, 0, 2548, -1, "", 1);// 138
            AddComplexComponent((BaseAddon)this, 22000, -2, 9, 0, 2548, -1, "", 1);// 139
            AddComplexComponent((BaseAddon)this, 22000, -2, 8, 0, 2548, -1, "", 1);// 140
            AddComplexComponent((BaseAddon)this, 22000, -3, 9, 0, 2548, -1, "", 1);// 141
            AddComplexComponent((BaseAddon)this, 22000, -3, 8, 0, 2548, -1, "", 1);// 142
            AddComplexComponent((BaseAddon)this, 22000, -4, 9, 0, 2548, -1, "", 1);// 143
            AddComplexComponent((BaseAddon)this, 22000, -4, 8, 0, 2548, -1, "", 1);// 144
            AddComplexComponent((BaseAddon)this, 22000, -5, 9, 0, 2548, -1, "", 1);// 145
            AddComplexComponent((BaseAddon)this, 22000, -5, 8, 0, 2548, -1, "", 1);// 146
            AddComplexComponent((BaseAddon)this, 22000, -6, 9, 0, 2548, -1, "", 1);// 147
            AddComplexComponent((BaseAddon)this, 22000, -6, 8, 0, 2548, -1, "", 1);// 148
            AddComplexComponent((BaseAddon)this, 22000, -7, 9, 0, 2548, -1, "", 1);// 149
            AddComplexComponent((BaseAddon)this, 22000, -7, 8, 0, 2548, -1, "", 1);// 150
            AddComplexComponent((BaseAddon)this, 22000, -8, 9, 0, 2548, -1, "", 1);// 151
            AddComplexComponent((BaseAddon)this, 22000, -8, 8, 0, 2548, -1, "", 1);// 152
            AddComplexComponent((BaseAddon)this, 8136, -3, 6, 0, 2548, -1, "", 1);// 154
            AddComplexComponent((BaseAddon)this, 8446, -2, 6, 0, 1920, -1, "", 1);// 155
            AddComplexComponent((BaseAddon)this, 22000, -3, 2, 0, 2548, -1, "", 1);// 156
            AddComplexComponent((BaseAddon)this, 22000, -1, 2, 0, 2548, -1, "", 1);// 157
            AddComplexComponent((BaseAddon)this, 22000, -2, 2, 0, 2548, -1, "", 1);// 158
            AddComplexComponent((BaseAddon)this, 22000, 7, 2, 0, 2548, -1, "", 1);// 159
            AddComplexComponent((BaseAddon)this, 22000, 7, 1, 0, 2548, -1, "", 1);// 160
            AddComplexComponent((BaseAddon)this, 22000, 7, 0, 0, 2548, -1, "", 1);// 161
            AddComplexComponent((BaseAddon)this, 22000, 7, -1, 0, 2548, -1, "", 1);// 162
            AddComplexComponent((BaseAddon)this, 22000, 7, -2, 0, 2548, -1, "", 1);// 163
            AddComplexComponent((BaseAddon)this, 22000, 7, -3, 0, 2548, -1, "", 1);// 164
            AddComplexComponent((BaseAddon)this, 22000, 6, 2, 0, 2548, -1, "", 1);// 165
            AddComplexComponent((BaseAddon)this, 22000, 6, 1, 0, 2548, -1, "", 1);// 166
            AddComplexComponent((BaseAddon)this, 22000, 6, 0, 0, 2548, -1, "", 1);// 167
            AddComplexComponent((BaseAddon)this, 22000, 6, -1, 0, 2548, -1, "", 1);// 168
            AddComplexComponent((BaseAddon)this, 22000, 6, -2, 0, 2548, -1, "", 1);// 169
            AddComplexComponent((BaseAddon)this, 22000, 6, -3, 0, 2548, -1, "", 1);// 170
            AddComplexComponent((BaseAddon)this, 22000, 5, 2, 0, 2548, -1, "", 1);// 171
            AddComplexComponent((BaseAddon)this, 22000, 5, 1, 0, 2548, -1, "", 1);// 172
            AddComplexComponent((BaseAddon)this, 22000, 5, 0, 0, 2548, -1, "", 1);// 173
            AddComplexComponent((BaseAddon)this, 22000, 5, -1, 0, 2548, -1, "", 1);// 174
            AddComplexComponent((BaseAddon)this, 22000, 7, 7, 0, 2548, -1, "", 1);// 175
            AddComplexComponent((BaseAddon)this, 22000, 7, 6, 0, 2548, -1, "", 1);// 176
            AddComplexComponent((BaseAddon)this, 22000, 7, 5, 0, 2548, -1, "", 1);// 177
            AddComplexComponent((BaseAddon)this, 22000, 7, 4, 0, 2548, -1, "", 1);// 178
            AddComplexComponent((BaseAddon)this, 22000, 7, 3, 0, 2548, -1, "", 1);// 179
            AddComplexComponent((BaseAddon)this, 22000, 6, 7, 0, 2548, -1, "", 1);// 180
            AddComplexComponent((BaseAddon)this, 22000, 6, 6, 0, 2548, -1, "", 1);// 181
            AddComplexComponent((BaseAddon)this, 22000, 6, 5, 0, 2548, -1, "", 1);// 182
            AddComplexComponent((BaseAddon)this, 22000, 6, 4, 0, 2548, -1, "", 1);// 183
            AddComplexComponent((BaseAddon)this, 22000, 6, 3, 0, 2548, -1, "", 1);// 184
            AddComplexComponent((BaseAddon)this, 22000, 5, 7, 0, 2548, -1, "", 1);// 185
            AddComplexComponent((BaseAddon)this, 22000, 5, 6, 0, 2548, -1, "", 1);// 186
            AddComplexComponent((BaseAddon)this, 22000, 5, 5, 0, 2548, -1, "", 1);// 187
            AddComplexComponent((BaseAddon)this, 22000, 5, 4, 0, 2548, -1, "", 1);// 188
            AddComplexComponent((BaseAddon)this, 22000, 5, 3, 0, 2548, -1, "", 1);// 189
            AddComplexComponent((BaseAddon)this, 22000, 4, 7, 0, 2548, -1, "", 1);// 190
            AddComplexComponent((BaseAddon)this, 22000, 4, 6, 0, 2548, -1, "", 1);// 191
            AddComplexComponent((BaseAddon)this, 22000, 4, 5, 0, 2548, -1, "", 1);// 192
            AddComplexComponent((BaseAddon)this, 22000, 4, 4, 0, 2548, -1, "", 1);// 193
            AddComplexComponent((BaseAddon)this, 22000, 4, 3, 0, 2548, -1, "", 1);// 194
            AddComplexComponent((BaseAddon)this, 22000, 3, 7, 0, 2548, -1, "", 1);// 195
            AddComplexComponent((BaseAddon)this, 22000, 3, 6, 0, 2548, -1, "", 1);// 196
            AddComplexComponent((BaseAddon)this, 22000, 3, 5, 0, 2548, -1, "", 1);// 197
            AddComplexComponent((BaseAddon)this, 22000, 3, 4, 0, 2548, -1, "", 1);// 198
            AddComplexComponent((BaseAddon)this, 22000, 3, 3, 0, 2548, -1, "", 1);// 199
            AddComplexComponent((BaseAddon)this, 22000, 2, 7, 0, 2548, -1, "", 1);// 200
            AddComplexComponent((BaseAddon)this, 22000, 2, 6, 0, 2548, -1, "", 1);// 201
            AddComplexComponent((BaseAddon)this, 22000, 2, 5, 0, 2548, -1, "", 1);// 202
            AddComplexComponent((BaseAddon)this, 22000, 2, 4, 0, 2548, -1, "", 1);// 203
            AddComplexComponent((BaseAddon)this, 22000, 2, 3, 0, 2548, -1, "", 1);// 204
            AddComplexComponent((BaseAddon)this, 22000, 1, 7, 0, 2548, -1, "", 1);// 205
            AddComplexComponent((BaseAddon)this, 22000, 1, 6, 0, 2548, -1, "", 1);// 206
            AddComplexComponent((BaseAddon)this, 22000, 1, 5, 0, 2548, -1, "", 1);// 207
            AddComplexComponent((BaseAddon)this, 22000, 1, 4, 0, 2548, -1, "", 1);// 208
            AddComplexComponent((BaseAddon)this, 22000, 1, 3, 0, 2548, -1, "", 1);// 209
            AddComplexComponent((BaseAddon)this, 22000, 0, 7, 0, 2548, -1, "", 1);// 210
            AddComplexComponent((BaseAddon)this, 22000, 0, 6, 0, 2548, -1, "", 1);// 211
            AddComplexComponent((BaseAddon)this, 22000, 0, 5, 0, 2548, -1, "", 1);// 212
            AddComplexComponent((BaseAddon)this, 22000, 0, 4, 0, 2548, -1, "", 1);// 213
            AddComplexComponent((BaseAddon)this, 22000, 0, 3, 0, 2548, -1, "", 1);// 214
            AddComplexComponent((BaseAddon)this, 22000, -1, 7, 0, 2548, -1, "", 1);// 215
            AddComplexComponent((BaseAddon)this, 22000, -1, 6, 0, 2548, -1, "", 1);// 216
            AddComplexComponent((BaseAddon)this, 22000, -1, 5, 0, 2548, -1, "", 1);// 217
            AddComplexComponent((BaseAddon)this, 22000, -1, 4, 0, 2548, -1, "", 1);// 218
            AddComplexComponent((BaseAddon)this, 22000, -1, 3, 0, 2548, -1, "", 1);// 219
            AddComplexComponent((BaseAddon)this, 22000, -2, 7, 0, 2548, -1, "", 1);// 220
            AddComplexComponent((BaseAddon)this, 22000, -2, 6, 0, 2548, -1, "", 1);// 221
            AddComplexComponent((BaseAddon)this, 22000, -2, 5, 0, 2548, -1, "", 1);// 222
            AddComplexComponent((BaseAddon)this, 22000, -2, 4, 0, 2548, -1, "", 1);// 223
            AddComplexComponent((BaseAddon)this, 22000, -2, 3, 0, 2548, -1, "", 1);// 224
            AddComplexComponent((BaseAddon)this, 22000, -3, 7, 0, 2548, -1, "", 1);// 225
            AddComplexComponent((BaseAddon)this, 22000, -3, 6, 0, 2548, -1, "", 1);// 226
            AddComplexComponent((BaseAddon)this, 22000, -3, 5, 0, 2548, -1, "", 1);// 227
            AddComplexComponent((BaseAddon)this, 22000, -3, 4, 0, 2548, -1, "", 1);// 228
            AddComplexComponent((BaseAddon)this, 22000, -3, 3, 0, 2548, -1, "", 1);// 229
            AddComplexComponent((BaseAddon)this, 22000, -4, 7, 0, 2548, -1, "", 1);// 230
            AddComplexComponent((BaseAddon)this, 22000, -4, 6, 0, 2548, -1, "", 1);// 231
            AddComplexComponent((BaseAddon)this, 22000, -4, 5, 0, 2548, -1, "", 1);// 232
            AddComplexComponent((BaseAddon)this, 22000, -4, 4, 0, 2548, -1, "", 1);// 233
            AddComplexComponent((BaseAddon)this, 22000, -4, 3, 0, 2548, -1, "", 1);// 234
            AddComplexComponent((BaseAddon)this, 22000, -5, 7, 0, 2548, -1, "", 1);// 235
            AddComplexComponent((BaseAddon)this, 22000, -5, 6, 0, 2548, -1, "", 1);// 236
            AddComplexComponent((BaseAddon)this, 22000, -5, 5, 0, 2548, -1, "", 1);// 237
            AddComplexComponent((BaseAddon)this, 22000, -5, 4, 0, 2548, -1, "", 1);// 238
            AddComplexComponent((BaseAddon)this, 22000, -5, 3, 0, 2548, -1, "", 1);// 239
            AddComplexComponent((BaseAddon)this, 22000, -5, 2, 0, 2548, -1, "", 1);// 240
            AddComplexComponent((BaseAddon)this, 22000, -6, 7, 0, 2548, -1, "", 1);// 241
            AddComplexComponent((BaseAddon)this, 22000, -6, 6, 0, 2548, -1, "", 1);// 242
            AddComplexComponent((BaseAddon)this, 22000, -6, 5, 0, 2548, -1, "", 1);// 243
            AddComplexComponent((BaseAddon)this, 22000, -6, 4, 0, 2548, -1, "", 1);// 244
            AddComplexComponent((BaseAddon)this, 22000, -6, 3, 0, 2548, -1, "", 1);// 245
            AddComplexComponent((BaseAddon)this, 22000, -6, 2, 0, 2548, -1, "", 1);// 246
            AddComplexComponent((BaseAddon)this, 22000, -7, 7, 0, 2548, -1, "", 1);// 247
            AddComplexComponent((BaseAddon)this, 22000, -7, 6, 0, 2548, -1, "", 1);// 248
            AddComplexComponent((BaseAddon)this, 22000, -7, 5, 0, 2548, -1, "", 1);// 249
            AddComplexComponent((BaseAddon)this, 22000, -7, 4, 0, 2548, -1, "", 1);// 250
            AddComplexComponent((BaseAddon)this, 22000, -7, 3, 0, 2548, -1, "", 1);// 251
            AddComplexComponent((BaseAddon)this, 22000, -7, 2, 0, 2548, -1, "", 1);// 252
            AddComplexComponent((BaseAddon)this, 22000, -8, 7, 0, 2548, -1, "", 1);// 253
            AddComplexComponent((BaseAddon)this, 22000, -8, 6, 0, 2548, -1, "", 1);// 254
            AddComplexComponent((BaseAddon)this, 22000, -8, 5, 0, 2548, -1, "", 1);// 255
            AddComplexComponent((BaseAddon)this, 22000, -8, 4, 0, 2548, -1, "", 1);// 256
            AddComplexComponent((BaseAddon)this, 22000, -8, 3, 0, 2548, -1, "", 1);// 257
            AddComplexComponent((BaseAddon)this, 22000, -8, 2, 0, 2548, -1, "", 1);// 258
            AddComplexComponent((BaseAddon)this, 22000, -6, 1, 0, 2548, -1, "", 1);// 259
            AddComplexComponent((BaseAddon)this, 22000, -6, 0, 0, 2548, -1, "", 1);// 260
            AddComplexComponent((BaseAddon)this, 22000, -6, -1, 0, 2548, -1, "", 1);// 261
            AddComplexComponent((BaseAddon)this, 22000, -6, -2, 0, 2548, -1, "", 1);// 262
            AddComplexComponent((BaseAddon)this, 22000, -6, -3, 0, 2548, -1, "", 1);// 263
            AddComplexComponent((BaseAddon)this, 22000, -6, -4, 0, 2548, -1, "", 1);// 264
            AddComplexComponent((BaseAddon)this, 22000, -6, -5, 0, 2548, -1, "", 1);// 265
            AddComplexComponent((BaseAddon)this, 22000, -7, 1, 0, 2548, -1, "", 1);// 266
            AddComplexComponent((BaseAddon)this, 22000, -7, 0, 0, 2548, -1, "", 1);// 267
            AddComplexComponent((BaseAddon)this, 22000, -7, -1, 0, 2548, -1, "", 1);// 268
            AddComplexComponent((BaseAddon)this, 22000, -7, -2, 0, 2548, -1, "", 1);// 269
            AddComplexComponent((BaseAddon)this, 22000, -7, -3, 0, 2548, -1, "", 1);// 270
            AddComplexComponent((BaseAddon)this, 22000, -7, -4, 0, 2548, -1, "", 1);// 271
            AddComplexComponent((BaseAddon)this, 22000, -7, -5, 0, 2548, -1, "", 1);// 272
            AddComplexComponent((BaseAddon)this, 22000, -8, 1, 0, 2548, -1, "", 1);// 273
            AddComplexComponent((BaseAddon)this, 22000, -8, 0, 0, 2548, -1, "", 1);// 274
            AddComplexComponent((BaseAddon)this, 22000, -8, -1, 0, 2548, -1, "", 1);// 275
            AddComplexComponent((BaseAddon)this, 22000, -8, -2, 0, 2548, -1, "", 1);// 276
            AddComplexComponent((BaseAddon)this, 22000, -8, -3, 0, 2548, -1, "", 1);// 277
            AddComplexComponent((BaseAddon)this, 22000, -8, -4, 0, 2548, -1, "", 1);// 278
            AddComplexComponent((BaseAddon)this, 22000, -8, -5, 0, 2548, -1, "", 1);// 279
            AddComplexComponent((BaseAddon)this, 22000, -6, -6, 0, 2548, -1, "", 1);// 280
            AddComplexComponent((BaseAddon)this, 22000, -6, -7, 0, 2548, -1, "", 1);// 281
            AddComplexComponent((BaseAddon)this, 22000, -6, -8, 0, 2548, -1, "", 1);// 282
            AddComplexComponent((BaseAddon)this, 22000, -7, -6, 0, 2548, -1, "", 1);// 283
            AddComplexComponent((BaseAddon)this, 22000, -7, -7, 0, 2548, -1, "", 1);// 284
            AddComplexComponent((BaseAddon)this, 22000, -7, -8, 0, 2548, -1, "", 1);// 285
            AddComplexComponent((BaseAddon)this, 22000, -8, -6, 0, 2548, -1, "", 1);// 286
            AddComplexComponent((BaseAddon)this, 22000, -8, -7, 0, 2548, -1, "", 1);// 287
            AddComplexComponent((BaseAddon)this, 22000, -8, -8, 0, 2548, -1, "", 1);// 288
            AddComplexComponent((BaseAddon)this, 14120, 4, -8, 0, 762, -1, "", 1);// 292
            AddComplexComponent((BaseAddon)this, 14120, 6, -8, 0, 762, -1, "", 1);// 293
            AddComplexComponent((BaseAddon)this, 14120, 7, -8, 0, 762, -1, "", 1);// 294
            AddComplexComponent((BaseAddon)this, 22000, -6, -9, 0, 2548, -1, "", 1);// 326
            AddComplexComponent((BaseAddon)this, 22000, -7, -9, 0, 2548, -1, "", 1);// 327
            AddComplexComponent((BaseAddon)this, 22000, -8, -9, 0, 2548, -1, "", 1);// 328
            AddComplexComponent((BaseAddon)this, 7897, 6, -9, 10, 2548, -1, "", 1);// 329
            AddComplexComponent((BaseAddon)this, 14120, 3, -9, 0, 762, -1, "", 1);// 330
            AddComplexComponent((BaseAddon)this, 14120, 2, -9, 0, 762, -1, "", 1);// 331
            AddComplexComponent((BaseAddon)this, 14120, 1, -9, 0, 762, -1, "", 1);// 332
            AddComplexComponent((BaseAddon)this, 14120, 0, -9, 0, 762, -1, "", 1);// 333
            AddComplexComponent((BaseAddon)this, 14120, -1, -9, 0, 762, -1, "", 1);// 334
            AddComplexComponent((BaseAddon)this, 22000, -9, 9, 0, 2548, -1, "", 1);// 337
            AddComplexComponent((BaseAddon)this, 22000, -9, 8, 0, 2548, -1, "", 1);// 338
            AddComplexComponent((BaseAddon)this, 22000, -10, 9, 0, 2548, -1, "", 1);// 339
            AddComplexComponent((BaseAddon)this, 22000, -10, 8, 0, 2548, -1, "", 1);// 340
            AddComplexComponent((BaseAddon)this, 22000, -11, 9, 0, 2548, -1, "", 1);// 341
            AddComplexComponent((BaseAddon)this, 22000, -11, 8, 0, 2548, -1, "", 1);// 342
            AddComplexComponent((BaseAddon)this, 22000, -12, 9, 0, 2548, -1, "", 1);// 343
            AddComplexComponent((BaseAddon)this, 22000, -12, 8, 0, 2548, -1, "", 1);// 344
            AddComplexComponent((BaseAddon)this, 22000, -13, 9, 0, 2548, -1, "", 1);// 345
            AddComplexComponent((BaseAddon)this, 22000, -13, 8, 0, 2548, -1, "", 1);// 346
            AddComplexComponent((BaseAddon)this, 22000, -9, 7, 0, 2548, -1, "", 1);// 350
            AddComplexComponent((BaseAddon)this, 22000, -9, 6, 0, 2548, -1, "", 1);// 351
            AddComplexComponent((BaseAddon)this, 22000, -9, 5, 0, 2548, -1, "", 1);// 352
            AddComplexComponent((BaseAddon)this, 22000, -9, 4, 0, 2548, -1, "", 1);// 353
            AddComplexComponent((BaseAddon)this, 22000, -9, 3, 0, 2548, -1, "", 1);// 354
            AddComplexComponent((BaseAddon)this, 22000, -9, 2, 0, 2548, -1, "", 1);// 355
            AddComplexComponent((BaseAddon)this, 22000, -10, 7, 0, 2548, -1, "", 1);// 356
            AddComplexComponent((BaseAddon)this, 22000, -10, 6, 0, 2548, -1, "", 1);// 357
            AddComplexComponent((BaseAddon)this, 22000, -10, 5, 0, 2548, -1, "", 1);// 358
            AddComplexComponent((BaseAddon)this, 22000, -10, 4, 0, 2548, -1, "", 1);// 359
            AddComplexComponent((BaseAddon)this, 22000, -10, 3, 0, 2548, -1, "", 1);// 360
            AddComplexComponent((BaseAddon)this, 22000, -10, 2, 0, 2548, -1, "", 1);// 361
            AddComplexComponent((BaseAddon)this, 22000, -11, 7, 0, 2548, -1, "", 1);// 362
            AddComplexComponent((BaseAddon)this, 22000, -11, 6, 0, 2548, -1, "", 1);// 363
            AddComplexComponent((BaseAddon)this, 22000, -11, 5, 0, 2548, -1, "", 1);// 364
            AddComplexComponent((BaseAddon)this, 22000, -11, 4, 0, 2548, -1, "", 1);// 365
            AddComplexComponent((BaseAddon)this, 22000, -11, 3, 0, 2548, -1, "", 1);// 366
            AddComplexComponent((BaseAddon)this, 22000, -11, 2, 0, 2548, -1, "", 1);// 367
            AddComplexComponent((BaseAddon)this, 22000, -12, 7, 0, 2548, -1, "", 1);// 368
            AddComplexComponent((BaseAddon)this, 22000, -12, 6, 0, 2548, -1, "", 1);// 369
            AddComplexComponent((BaseAddon)this, 22000, -12, 5, 0, 2548, -1, "", 1);// 370
            AddComplexComponent((BaseAddon)this, 22000, -12, 4, 0, 2548, -1, "", 1);// 371
            AddComplexComponent((BaseAddon)this, 22000, -12, 3, 0, 2548, -1, "", 1);// 372
            AddComplexComponent((BaseAddon)this, 22000, -12, 2, 0, 2548, -1, "", 1);// 373
            AddComplexComponent((BaseAddon)this, 22000, -13, 7, 0, 2548, -1, "", 1);// 374
            AddComplexComponent((BaseAddon)this, 22000, -13, 6, 0, 2548, -1, "", 1);// 375
            AddComplexComponent((BaseAddon)this, 22000, -13, 5, 0, 2548, -1, "", 1);// 376
            AddComplexComponent((BaseAddon)this, 22000, -13, 4, 0, 2548, -1, "", 1);// 377
            AddComplexComponent((BaseAddon)this, 22000, -13, 3, 0, 2548, -1, "", 1);// 378
            AddComplexComponent((BaseAddon)this, 22000, -13, 2, 0, 2548, -1, "", 1);// 379
            AddComplexComponent((BaseAddon)this, 22000, -9, 1, 0, 2548, -1, "", 1);// 380
            AddComplexComponent((BaseAddon)this, 22000, -9, 0, 0, 2548, -1, "", 1);// 381
            AddComplexComponent((BaseAddon)this, 22000, -9, -1, 0, 2548, -1, "", 1);// 382
            AddComplexComponent((BaseAddon)this, 22000, -9, -2, 0, 2548, -1, "", 1);// 383
            AddComplexComponent((BaseAddon)this, 22000, -9, -3, 0, 2548, -1, "", 1);// 384
            AddComplexComponent((BaseAddon)this, 22000, -9, -4, 0, 2548, -1, "", 1);// 385
            AddComplexComponent((BaseAddon)this, 22000, -9, -5, 0, 2548, -1, "", 1);// 386
            AddComplexComponent((BaseAddon)this, 22000, -10, 1, 0, 2548, -1, "", 1);// 387
            AddComplexComponent((BaseAddon)this, 22000, -10, 0, 0, 2548, -1, "", 1);// 388
            AddComplexComponent((BaseAddon)this, 22000, -10, -1, 0, 2548, -1, "", 1);// 389
            AddComplexComponent((BaseAddon)this, 22000, -10, -2, 0, 2548, -1, "", 1);// 390
            AddComplexComponent((BaseAddon)this, 22000, -10, -3, 0, 2548, -1, "", 1);// 391
            AddComplexComponent((BaseAddon)this, 22000, -10, -4, 0, 2548, -1, "", 1);// 392
            AddComplexComponent((BaseAddon)this, 22000, -10, -5, 0, 2548, -1, "", 1);// 393
            AddComplexComponent((BaseAddon)this, 22000, -11, 1, 0, 2548, -1, "", 1);// 394
            AddComplexComponent((BaseAddon)this, 22000, -11, 0, 0, 2548, -1, "", 1);// 395
            AddComplexComponent((BaseAddon)this, 22000, -11, -1, 0, 2548, -1, "", 1);// 396
            AddComplexComponent((BaseAddon)this, 22000, -11, -2, 0, 2548, -1, "", 1);// 397
            AddComplexComponent((BaseAddon)this, 22000, -11, -3, 0, 2548, -1, "", 1);// 398
            AddComplexComponent((BaseAddon)this, 22000, -11, -4, 0, 2548, -1, "", 1);// 399
            AddComplexComponent((BaseAddon)this, 22000, -11, -5, 0, 2548, -1, "", 1);// 400
            AddComplexComponent((BaseAddon)this, 22000, -12, 1, 0, 2548, -1, "", 1);// 401
            AddComplexComponent((BaseAddon)this, 22000, -12, 0, 0, 2548, -1, "", 1);// 402
            AddComplexComponent((BaseAddon)this, 22000, -12, -1, 0, 2548, -1, "", 1);// 403
            AddComplexComponent((BaseAddon)this, 22000, -12, -2, 0, 2548, -1, "", 1);// 404
            AddComplexComponent((BaseAddon)this, 22000, -12, -3, 0, 2548, -1, "", 1);// 405
            AddComplexComponent((BaseAddon)this, 22000, -12, -4, 0, 2548, -1, "", 1);// 406
            AddComplexComponent((BaseAddon)this, 22000, -12, -5, 0, 2548, -1, "", 1);// 407
            AddComplexComponent((BaseAddon)this, 22000, -13, 1, 0, 2548, -1, "", 1);// 408
            AddComplexComponent((BaseAddon)this, 22000, -13, 0, 0, 2548, -1, "", 1);// 409
            AddComplexComponent((BaseAddon)this, 22000, -13, -1, 0, 2548, -1, "", 1);// 410
            AddComplexComponent((BaseAddon)this, 22000, -13, -2, 0, 2548, -1, "", 1);// 411
            AddComplexComponent((BaseAddon)this, 22000, -13, -3, 0, 2548, -1, "", 1);// 412
            AddComplexComponent((BaseAddon)this, 22000, -13, -4, 0, 2548, -1, "", 1);// 413
            AddComplexComponent((BaseAddon)this, 22000, -13, -5, 0, 2548, -1, "", 1);// 414
            AddComplexComponent((BaseAddon)this, 22000, -9, -6, 0, 2548, -1, "", 1);// 415
            AddComplexComponent((BaseAddon)this, 22000, -9, -7, 0, 2548, -1, "", 1);// 416
            AddComplexComponent((BaseAddon)this, 22000, -9, -8, 0, 2548, -1, "", 1);// 417
            AddComplexComponent((BaseAddon)this, 22000, -10, -6, 0, 2548, -1, "", 1);// 418
            AddComplexComponent((BaseAddon)this, 22000, -10, -7, 0, 2548, -1, "", 1);// 419
            AddComplexComponent((BaseAddon)this, 22000, -10, -8, 0, 2548, -1, "", 1);// 420
            AddComplexComponent((BaseAddon)this, 22000, -11, -6, 0, 2548, -1, "", 1);// 421
            AddComplexComponent((BaseAddon)this, 22000, -11, -7, 0, 2548, -1, "", 1);// 422
            AddComplexComponent((BaseAddon)this, 22000, -11, -8, 0, 2548, -1, "", 1);// 423
            AddComplexComponent((BaseAddon)this, 22000, -12, -6, 0, 2548, -1, "", 1);// 424
            AddComplexComponent((BaseAddon)this, 22000, -12, -7, 0, 2548, -1, "", 1);// 425
            AddComplexComponent((BaseAddon)this, 22000, -12, -8, 0, 2548, -1, "", 1);// 426
            AddComplexComponent((BaseAddon)this, 22000, -13, -6, 0, 2548, -1, "", 1);// 427
            AddComplexComponent((BaseAddon)this, 22000, -13, -7, 0, 2548, -1, "", 1);// 428
            AddComplexComponent((BaseAddon)this, 22000, -13, -8, 0, 2548, -1, "", 1);// 429
            AddComplexComponent((BaseAddon)this, 22000, -9, -9, 0, 2548, -1, "", 1);// 430
            AddComplexComponent((BaseAddon)this, 22000, -10, -9, 0, 2548, -1, "", 1);// 431
            AddComplexComponent((BaseAddon)this, 22000, -11, -9, 0, 2548, -1, "", 1);// 432
            AddComplexComponent((BaseAddon)this, 22000, -12, -9, 0, 2548, -1, "", 1);// 433
            AddComplexComponent((BaseAddon)this, 22000, -13, -9, 0, 2548, -1, "", 1);// 434
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
