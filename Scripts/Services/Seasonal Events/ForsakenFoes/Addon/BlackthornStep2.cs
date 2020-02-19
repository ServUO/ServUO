using System;

namespace Server.Items
{
    public class BlackthornStep2 : BlackthornBaseAddon
    {
        public static BlackthornStep2 InstanceTram { get; set; }
        public static BlackthornStep2 InstanceFel { get; set; }

        private static int[,] m_AddOnSimpleComponents = new int[,]
        {
              {15874, 6, 13, 0}, {15874, 7, 13, 0}, {15874, 8, 13, 0}// 6	7	8	
			, {15874, 9, 13, 0}, {15874, 10, 13, 0}, {15874, 11, 13, 0}// 9	10	11	
			, {6584, 20, 15, 0}, {6584, 21, 16, 1}, {17729, 13, 13, 0}// 18	22	53	
			, {41325, 6, -2, 6}, {15874, 6, 5, 0}, {15874, 7, 5, 0}// 54	55	56	
			, {15874, 8, 5, 0}, {15874, 9, 5, 0}, {15874, 10, 5, 0}// 57	58	59	
			, {15874, 11, 5, 0}, {4309, 6, -8, 6}, {17081, -1, 19, 0}// 60	152	153	
			, {17077, 5, 25, 12}, {17088, 1, 25, 13}, {40737, 4, 26, 1}// 154	155	156	
			, {17082, -1, 26, 0}, {40735, -1, 24, 1}, {40737, -1, 20, 1}// 157	158	159	
			, {16388, 1, 21, 13}, {17083, 5, 21, 13}, {40738, 4, 19, 1}// 160	161	162	
			, {16649, 1, 17, 0}, {15874, -4, 13, 0}, {15874, -3, 13, 0}// 163	164	165	
			, {15874, -2, 13, 0}, {15874, -5, 13, 0}, {15874, -6, 13, 0}// 166	167	168	
			, {4306, 0, -2, 2}, {4306, 2, -4, 5}, {41195, 1, 1, 0}// 171	172	173	
			, {41156, 0, 7, 0}, {12906, -10, 9, 0}, {12906, -9, -4, 0}// 174	175	179	
			, {12906, -9, -3, 0}, {12906, -10, -4, 0}, {12906, -10, -3, 0}// 180	181	182	
			, {12906, -9, -2, 0}, {12906, -10, -2, 0}, {12906, -10, -1, 0}// 183	184	185	
			, {12906, -9, -1, 0}, {12906, -9, 0, 0}, {12906, -9, 1, 0}// 186	187	188	
			, {12906, -10, 0, 0}, {12906, -10, 1, 0}, {12906, -8, 3, 0}// 189	190	191	
			, {12906, -9, 2, 0}, {12906, -10, 2, 0}, {12906, -10, 3, 0}// 192	193	194	
			, {12906, -9, 3, 0}, {12906, -9, 4, 0}, {12906, -8, 4, 0}// 195	196	197	
			, {4963, -10, 4, 0}, {4967, -7, 5, 0}, {15874, -6, 5, 0}// 199	205	206	
			, {15874, -5, 5, 0}, {15874, -4, 5, 0}, {15874, -3, 5, 0}// 207	208	209	
			, {15874, -2, 5, 0}, {15874, -1, 5, 0}, {15874, 0, 5, 0}// 210	211	212	
			, {15874, 1, 4, 0}, {15874, 2, 4, 0}, {15719, 2, -5, 6}// 213	214	215	
			, {4317, -1, -10, 2}, {4313, -1, -10, 0}, {41317, 2, -9, 6}// 216	217	218	
			, {16642, 4, -10, 0}, {12906, -9, -20, 0}, {12906, -10, -20, 0}// 219	220	221	
			, {12906, -10, -19, 0}, {12906, -9, -19, 0}, {12906, -9, -18, 0}// 222	223	224	
			, {12906, -10, -18, 0}, {12906, -10, -17, 0}, {12906, -9, -17, 0}// 225	226	227	
			, {12906, -9, -16, 0}, {12906, -10, -16, 0}, {12906, -10, -15, 0}// 228	229	230	
			, {12906, -9, -15, 0}, {12906, -9, -14, 0}, {12906, -10, -14, 0}// 231	232	233	
			, {12906, -10, -13, 0}, {12906, -9, -13, 0}, {12906, -9, -12, 0}// 234	235	236	
			, {12906, -10, -12, 0}, {12906, -10, -11, 0}, {12906, -9, -11, 0}// 237	238	239	
			, {12906, -9, -10, 0}, {12906, -10, -10, 0}, {12906, -10, -8, 0}// 240	241	242	
			, {12906, -9, -8, 0}, {12906, -9, -9, 0}, {12906, -10, -9, 0}// 243	244	245	
			, {12906, -10, -7, 0}, {12906, -9, -7, 0}, {12906, -9, -6, 0}// 246	247	248	
			, {12906, -10, -6, 0}, {12906, -10, -5, 0}, {12906, -9, -5, 0}// 249	250	251	
			, {4410, -9, -22, 0}, {12906, -10, -21, 0}, {12906, -10, -22, 0}// 252	253	254	
			, {12906, -9, -22, 0}, {12906, -9, -21, 0}, {12906, -9, -23, 0}// 255	256	257	
			, {12906, -10, -23, 0}, {12906, -10, -24, 0}, {12906, -9, -24, 0}// 258	259	260	
			, {12906, -9, -25, 0}, {12906, -10, -25, 0}, {12906, -11, 12, 0}// 261	262	263	
			, {12906, -12, 12, 0}, {12906, -13, 12, 0}, {12906, -14, 12, 0}// 264	265	266	
			, {12906, -15, 12, 0}, {12906, -16, 12, 0}, {12906, -17, 12, 0}// 267	268	269	
			, {12906, -18, 12, 0}, {12906, -19, 12, 0}, {12906, -20, 12, 0}// 270	271	272	
			, {12906, -21, 12, 0}, {12906, -22, 12, 0}, {40364, -16, 9, 0}// 273	274	275	
			, {19100, -18, 9, 0}, {15800, -14, 11, 0}, {12865, -19, -4, 0}// 276	277	278	
			, {12865, -21, -3, 0}, {12865, -21, -1, 0}, {12865, -20, 0, 0}// 279	280	281	
			, {12865, -20, 3, 0}, {12865, -21, 2, 0}, {12865, -21, 5, 0}// 282	283	284	
			, {12865, -19, 4, 0}, {12865, -20, 7, 0}, {12865, -21, 8, 0}// 285	286	287	
			, {12865, -20, 9, 0}, {12865, -20, 10, 0}, {12865, -21, 10, 0}// 288	289	290	
			, {12906, -11, 10, 0}, {12906, -12, 11, 0}, {12906, -11, 11, 0}// 291	292	293	
			, {12906, -13, 9, 0}, {12906, -12, 10, 0}, {12906, -13, 11, 0}// 294	295	296	
			, {12906, -14, 11, 0}, {12906, -14, 10, 0}, {12906, -14, 9, 0}// 297	298	299	
			, {12906, -13, 10, 0}, {12906, -15, 10, 0}, {12906, -15, 11, 0}// 300	301	302	
			, {12906, -16, 11, 0}, {12906, -16, 10, 0}, {12906, -17, 10, 0}// 303	304	305	
			, {12906, -17, 11, 0}, {12906, -17, 8, 0}, {12906, -16, 9, 0}// 306	307	308	
			, {12906, -17, 9, 0}, {12906, -18, 8, 0}, {12906, -18, 9, 0}// 309	310	311	
			, {12906, -18, 9, 0}, {12906, -19, 7, 0}, {12906, -19, 6, 0}// 312	313	314	
			, {12906, -19, 5, 0}, {12906, -19, 4, 0}, {12906, -19, 3, 0}// 315	316	317	
			, {12906, -19, 2, 0}, {12906, -19, 1, 0}, {12906, -19, 0, 0}// 318	319	320	
			, {12906, -19, -1, 0}, {12906, -19, -2, 0}, {12906, -19, -3, 0}// 321	322	323	
			, {12906, -19, -4, 0}, {12906, -20, -4, 0}, {12906, -21, -4, 0}// 324	325	326	
			, {12906, -22, -4, 0}, {12906, -22, -3, 0}, {12906, -21, -3, 0}// 327	328	329	
			, {12906, -20, -3, 0}, {12906, -20, -2, 0}, {12906, -20, -1, 0}// 330	331	332	
			, {12906, -20, 0, 0}, {12906, -20, 1, 0}, {12906, -20, 2, 0}// 333	334	335	
			, {12906, -20, 3, 0}, {12906, -20, 4, 0}, {12906, -20, 5, 0}// 336	337	338	
			, {12906, -20, 6, 0}, {12906, -20, 7, 0}, {12906, -20, 9, 0}// 339	340	341	
			, {12906, -20, 8, 0}, {12906, -19, 8, 0}, {12906, -19, 9, 0}// 342	343	344	
			, {12906, -19, 11, 0}, {12906, -19, 10, 0}, {12906, -20, 10, 0}// 345	346	347	
			, {12906, -20, 11, 0}, {12906, -21, 11, 0}, {12906, -21, 10, 0}// 348	349	350	
			, {12906, -21, 9, 0}, {12906, -21, 8, 0}, {12906, -21, 7, 0}// 351	352	353	
			, {12906, -21, 6, 0}, {12906, -21, 5, 0}, {12906, -21, 4, 0}// 354	355	356	
			, {12906, -21, 3, 0}, {12906, -21, 2, 0}, {12906, -21, 1, 0}// 357	358	359	
			, {12906, -21, 0, 0}, {12906, -21, -1, 0}, {12906, -21, -2, 0}// 360	361	362	
			, {12906, -22, -2, 0}, {12906, -22, -1, 0}, {12906, -22, 0, 0}// 363	364	365	
			, {12906, -22, 1, 0}, {12906, -22, 2, 0}, {12906, -22, 3, 0}// 366	367	368	
			, {12906, -22, 4, 0}, {12906, -22, 5, 0}, {12906, -22, 6, 0}// 369	370	371	
			, {12906, -22, 7, 0}, {12906, -22, 8, 0}, {12906, -22, 9, 0}// 372	373	374	
			, {12906, -22, 10, 0}, {12906, -22, 11, 0}, {12906, -18, 10, 0}// 375	376	377	
			, {12906, -18, 11, 0}, {4963, -18, 3, 0}, {4963, -18, 7, 0}// 378	392	396	
			, {12906, -11, -3, 0}, {12906, -11, -4, 0}, {12906, -12, -4, 0}// 406	407	408	
			, {12906, -12, -3, 0}, {12906, -13, -4, 0}, {12906, -14, -4, 0}// 409	410	411	
			, {12906, -14, -3, 0}, {12906, -13, -3, 0}, {12906, -11, -2, 0}// 412	413	414	
			, {12906, -12, -2, 0}, {12906, -12, -1, 0}, {12906, -11, -1, 0}// 415	416	417	
			, {12906, -11, 0, 0}, {12906, -11, 1, 0}, {12906, -12, 1, 0}// 418	419	420	
			, {12906, -12, 0, 0}, {12906, -14, -2, 0}, {12906, -13, -2, 0}// 421	422	423	
			, {12906, -14, -1, 0}, {12906, -13, -1, 0}, {12906, -14, 0, 0}// 424	425	426	
			, {12906, -13, 0, 0}, {12906, -14, 1, 0}, {12906, -13, 1, 0}// 427	428	429	
			, {12906, -13, 1, 0}, {12906, -12, 2, 0}, {12906, -11, 2, 0}// 430	431	432	
			, {4969, -14, -3, 0}, {4971, -14, -1, 0}, {12865, -21, -5, 0}// 437	438	451	
			, {12906, -20, -13, 0}, {12906, -20, -14, 0}, {12906, -20, -15, 0}// 452	453	454	
			, {12906, -20, -16, 0}, {12906, -20, -17, 0}, {12906, -20, -18, 0}// 455	456	457	
			, {12906, -20, -19, 0}, {12906, -20, -20, 0}, {12906, -21, -20, 0}// 458	459	460	
			, {12906, -21, -19, 0}, {12906, -21, -19, 0}, {12906, -21, -18, 0}// 461	462	463	
			, {12906, -21, -17, 0}, {12906, -21, -16, 0}, {12906, -21, -15, 0}// 464	465	466	
			, {12906, -21, -14, 0}, {12906, -21, -13, 0}, {12906, -22, -13, 0}// 467	468	469	
			, {12906, -22, -14, 0}, {12906, -22, -15, 0}, {12906, -22, -16, 0}// 470	471	472	
			, {12906, -22, -17, 0}, {12906, -22, -18, 0}, {12906, -22, -19, 0}// 473	474	475	
			, {12906, -22, -20, 0}, {12906, -19, -12, 0}, {12906, -20, -12, 0}// 476	477	478	
			, {12906, -21, -12, 0}, {12906, -22, -12, 0}, {12906, -22, -11, 0}// 479	480	481	
			, {12906, -21, -11, 0}, {12906, -20, -11, 0}, {12906, -19, -11, 0}// 482	483	484	
			, {12906, -19, -10, 0}, {12906, -20, -10, 0}, {12906, -21, -10, 0}// 485	486	487	
			, {12906, -22, -10, 0}, {12906, -22, -9, 0}, {12906, -21, -9, 0}// 488	489	490	
			, {12906, -20, -9, 0}, {12906, -20, -9, 0}, {12906, -19, -9, 0}// 491	492	493	
			, {12906, -19, -8, 0}, {12906, -20, -8, 0}, {12906, -21, -8, 0}// 494	495	496	
			, {12906, -22, -8, 0}, {12906, -22, -7, 0}, {12906, -21, -7, 0}// 497	498	499	
			, {12906, -20, -7, 0}, {12906, -19, -7, 0}, {12906, -19, -6, 0}// 500	501	502	
			, {12906, -20, -6, 0}, {12906, -21, -6, 0}, {12906, -22, -6, 0}// 503	504	505	
			, {12906, -22, -5, 0}, {12906, -21, -5, 0}, {12906, -20, -5, 0}// 506	507	508	
			, {12906, -19, -5, 0}, {19121, -12, -16, 0}, {12906, -14, -12, 0}// 509	532	533	
			, {12906, -14, -11, 0}, {12906, -14, -15, 0}, {12906, -14, -16, 0}// 534	535	536	
			, {12906, -14, -19, 0}, {12906, -13, -19, 0}, {12906, -14, -20, 0}// 537	538	539	
			, {12906, -11, -10, 0}, {12906, -11, -11, 0}, {12906, -11, -12, 0}// 540	541	542	
			, {12906, -11, -13, 0}, {12906, -11, -14, 0}, {12906, -11, -15, 0}// 543	544	545	
			, {12906, -11, -16, 0}, {12906, -11, -17, 0}, {12906, -11, -18, 0}// 546	547	548	
			, {12906, -11, -19, 0}, {12906, -11, -20, 0}, {12906, -12, -20, 0}// 549	550	551	
			, {12906, -12, -19, 0}, {12906, -12, -18, 0}, {12906, -12, -17, 0}// 552	553	554	
			, {12906, -12, -16, 0}, {12906, -12, -15, 0}, {12906, -12, -14, 0}// 555	556	557	
			, {12906, -12, -13, 0}, {12906, -12, -12, 0}, {12906, -12, -11, 0}// 558	559	560	
			, {12906, -12, -10, 0}, {12906, -13, -20, 0}, {12906, -13, -19, 0}// 561	562	563	
			, {12906, -13, -18, 0}, {12906, -13, -17, 0}, {12906, -13, -16, 0}// 564	565	566	
			, {12906, -13, -15, 0}, {12906, -13, -14, 0}, {12906, -13, -13, 0}// 567	568	569	
			, {12906, -13, -12, 0}, {12906, -13, -11, 0}, {12906, -13, -10, 0}// 570	571	572	
			, {12906, -11, -9, 0}, {12906, -12, -9, 0}, {12906, -13, -9, 0}// 573	574	575	
			, {12906, -14, -9, 0}, {12906, -14, -8, 0}, {12906, -13, -8, 0}// 576	577	578	
			, {12906, -12, -8, 0}, {12906, -11, -8, 0}, {12906, -11, -7, 0}// 579	580	581	
			, {12906, -11, -6, 0}, {12906, -12, -6, 0}, {12906, -12, -7, 0}// 582	583	584	
			, {12906, -13, -7, 0}, {12906, -14, -7, 0}, {12906, -14, -5, 0}// 585	586	587	
			, {12906, -13, -6, 0}, {12906, -13, -5, 0}, {12906, -12, -5, 0}// 588	589	590	
			, {12906, -11, -5, 0}, {4966, -14, -6, 0}, {4971, -14, -14, 0}// 591	604	613	
			, {4971, -14, -13, 0}, {12906, -22, -21, 0}, {12906, -21, -21, 0}// 614	624	625	
			, {12906, -21, -22, 0}, {12906, -22, -22, 0}, {12906, -22, -23, 0}// 626	627	628	
			, {12906, -21, -24, 0}, {12906, -21, -23, 0}, {12906, -22, -24, 0}// 629	630	631	
			, {12906, -22, -25, 0}, {15716, -12, -21, 0}, {12906, -14, -23, 0}// 632	641	642	
			, {12906, -14, -24, 0}, {12906, -11, -21, 0}, {12906, -11, -22, 0}// 643	644	645	
			, {12906, -12, -22, 0}, {12906, -12, -21, 0}, {12906, -11, -23, 0}// 646	647	648	
			, {12906, -12, -23, 0}, {12906, -12, -24, 0}, {12906, -11, -24, 0}// 649	650	651	
			, {12906, -11, -25, 0}, {12906, -12, -25, 0}, {12906, -13, -25, 0}// 652	653	654	
			, {12906, -13, -24, 0}, {12906, -13, -23, 0}, {12906, -13, -22, 0}// 655	656	657	
			, {12906, -13, -21, 0}, {4971, -14, -22, 0}// 658	667	
		};

        [Constructable]
        public BlackthornStep2()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent((BaseAddon)this, 22000, 22, 11, 0, 667, -1, "", 1);// 1
            AddComplexComponent((BaseAddon)this, 22000, 22, 10, 0, 667, -1, "", 1);// 2
            AddComplexComponent((BaseAddon)this, 22000, 22, 9, 0, 667, -1, "", 1);// 3
            AddComplexComponent((BaseAddon)this, 22000, 22, 7, 0, 667, -1, "", 1);// 4
            AddComplexComponent((BaseAddon)this, 22000, 22, 8, 0, 667, -1, "", 1);// 5
            AddComplexComponent((BaseAddon)this, 6585, 19, 16, 1, 2721, -1, "", 1);// 12
            AddComplexComponent((BaseAddon)this, 6585, 20, 16, 1, 2721, -1, "", 1);// 13
            AddComplexComponent((BaseAddon)this, 6585, 21, 16, 1, 2721, -1, "", 1);// 14
            AddComplexComponent((BaseAddon)this, 6585, 21, 16, 0, 2721, -1, "", 1);// 15
            AddComplexComponent((BaseAddon)this, 6585, 20, 16, 1, 2721, -1, "", 1);// 16
            AddComplexComponent((BaseAddon)this, 6585, 20, 16, 0, 2721, -1, "", 1);// 17
            AddComplexComponent((BaseAddon)this, 6584, 19, 16, 0, 2721, -1, "", 1);// 19
            AddComplexComponent((BaseAddon)this, 6584, 20, 16, 0, 2721, -1, "", 1);// 20
            AddComplexComponent((BaseAddon)this, 6584, 21, 16, 1, 2721, -1, "", 1);// 21
            AddComplexComponent((BaseAddon)this, 6586, 21, 16, 0, 2721, -1, "", 1);// 23
            AddComplexComponent((BaseAddon)this, 6586, 20, 16, 0, 2721, -1, "", 1);// 24
            AddComplexComponent((BaseAddon)this, 6586, 19, 16, 0, 2721, -1, "", 1);// 25
            AddComplexComponent((BaseAddon)this, 6585, 21, 15, 0, 2721, -1, "", 1);// 26
            AddComplexComponent((BaseAddon)this, 6585, 20, 15, 0, 2721, -1, "", 1);// 27
            AddComplexComponent((BaseAddon)this, 6585, 19, 15, 0, 2721, -1, "", 1);// 28
            AddComplexComponent((BaseAddon)this, 17738, 13, 12, 0, 1910, -1, "", 1);// 29
            AddComplexComponent((BaseAddon)this, 22000, 21, 12, 0, 667, -1, "", 1);// 30
            AddComplexComponent((BaseAddon)this, 22000, 21, 13, 0, 667, -1, "", 1);// 31
            AddComplexComponent((BaseAddon)this, 22000, 21, 14, 0, 667, -1, "", 1);// 32
            AddComplexComponent((BaseAddon)this, 22000, 16, 13, 0, 667, -1, "", 1);// 33
            AddComplexComponent((BaseAddon)this, 22000, 15, 13, 0, 667, -1, "", 1);// 34
            AddComplexComponent((BaseAddon)this, 22000, 14, 13, 0, 667, -1, "", 1);// 35
            AddComplexComponent((BaseAddon)this, 22000, 20, 12, 0, 667, -1, "", 1);// 36
            AddComplexComponent((BaseAddon)this, 22000, 19, 12, 0, 667, -1, "", 1);// 37
            AddComplexComponent((BaseAddon)this, 22000, 19, 13, 0, 667, -1, "", 1);// 38
            AddComplexComponent((BaseAddon)this, 22000, 20, 13, 0, 667, -1, "", 1);// 39
            AddComplexComponent((BaseAddon)this, 22000, 20, 14, 0, 667, -1, "", 1);// 40
            AddComplexComponent((BaseAddon)this, 22000, 19, 14, 0, 667, -1, "", 1);// 41
            AddComplexComponent((BaseAddon)this, 22000, 18, 14, 0, 667, -1, "", 1);// 42
            AddComplexComponent((BaseAddon)this, 22000, 18, 13, 0, 667, -1, "", 1);// 43
            AddComplexComponent((BaseAddon)this, 22000, 18, 12, 0, 667, -1, "", 1);// 44
            AddComplexComponent((BaseAddon)this, 22000, 17, 13, 0, 667, -1, "", 1);// 45
            AddComplexComponent((BaseAddon)this, 22000, 17, 12, 0, 667, -1, "", 1);// 46
            AddComplexComponent((BaseAddon)this, 22000, 16, 12, 0, 667, -1, "", 1);// 47
            AddComplexComponent((BaseAddon)this, 22000, 15, 12, 0, 667, -1, "", 1);// 48
            AddComplexComponent((BaseAddon)this, 22000, 14, 12, 0, 667, -1, "", 1);// 49
            AddComplexComponent((BaseAddon)this, 17729, 13, 13, 5, 1910, -1, "", 1);// 50
            AddComplexComponent((BaseAddon)this, 17724, 13, 12, 5, 1910, -1, "", 1);// 51
            AddComplexComponent((BaseAddon)this, 17729, 13, 12, 0, 1910, -1, "", 1);// 52
            AddComplexComponent((BaseAddon)this, 6585, 21, 4, 0, 2721, -1, "", 1);// 61
            AddComplexComponent((BaseAddon)this, 6585, 21, 5, 0, 2721, -1, "", 1);// 62
            AddComplexComponent((BaseAddon)this, 6585, 20, 4, 2, 2721, -1, "", 1);// 63
            AddComplexComponent((BaseAddon)this, 6584, 20, 4, 1, 2721, -1, "", 1);// 64
            AddComplexComponent((BaseAddon)this, 6584, 20, 3, 0, 2721, -1, "", 1);// 65
            AddComplexComponent((BaseAddon)this, 6584, 20, 4, 1, 2721, -1, "", 1);// 66
            AddComplexComponent((BaseAddon)this, 6584, 19, 4, 0, 2721, -1, "", 1);// 67
            AddComplexComponent((BaseAddon)this, 6584, 19, 5, 1, 2721, -1, "", 1);// 68
            AddComplexComponent((BaseAddon)this, 6585, 19, 5, 0, 2721, -1, "", 1);// 69
            AddComplexComponent((BaseAddon)this, 6585, 19, 4, 0, 2721, -1, "", 1);// 70
            AddComplexComponent((BaseAddon)this, 6585, 20, 5, 0, 2721, -1, "", 1);// 71
            AddComplexComponent((BaseAddon)this, 6585, 19, 4, 0, 2721, -1, "", 1);// 72
            AddComplexComponent((BaseAddon)this, 6571, 19, 10, 0, 0, 1, "", 1);// 73
            AddComplexComponent((BaseAddon)this, 6571, 14, 6, 0, 0, 1, "", 1);// 74
            AddComplexComponent((BaseAddon)this, 40832, 18, 10, 0, 2076, -1, "", 1);// 75
            AddComplexComponent((BaseAddon)this, 22000, 21, 10, 0, 667, -1, "", 1);// 76
            AddComplexComponent((BaseAddon)this, 22000, 21, 11, 0, 667, -1, "", 1);// 77
            AddComplexComponent((BaseAddon)this, 22000, 21, 5, 0, 667, -1, "", 1);// 78
            AddComplexComponent((BaseAddon)this, 22000, 19, 7, 0, 667, -1, "", 1);// 79
            AddComplexComponent((BaseAddon)this, 22000, 19, 8, 0, 667, -1, "", 1);// 80
            AddComplexComponent((BaseAddon)this, 22000, 20, 8, 0, 667, -1, "", 1);// 81
            AddComplexComponent((BaseAddon)this, 22000, 20, 7, 0, 667, -1, "", 1);// 82
            AddComplexComponent((BaseAddon)this, 22000, 21, 6, 0, 667, -1, "", 1);// 83
            AddComplexComponent((BaseAddon)this, 22000, 21, 7, 0, 667, -1, "", 1);// 84
            AddComplexComponent((BaseAddon)this, 22000, 21, 8, 0, 667, -1, "", 1);// 85
            AddComplexComponent((BaseAddon)this, 22000, 21, 9, 0, 667, -1, "", 1);// 86
            AddComplexComponent((BaseAddon)this, 22000, 20, 9, 0, 667, -1, "", 1);// 87
            AddComplexComponent((BaseAddon)this, 22000, 19, 9, 0, 667, -1, "", 1);// 88
            AddComplexComponent((BaseAddon)this, 22000, 20, 10, 0, 667, -1, "", 1);// 89
            AddComplexComponent((BaseAddon)this, 22000, 19, 10, 0, 667, -1, "", 1);// 90
            AddComplexComponent((BaseAddon)this, 22000, 19, 11, 0, 667, -1, "", 1);// 91
            AddComplexComponent((BaseAddon)this, 22000, 20, 11, 0, 667, -1, "", 1);// 92
            AddComplexComponent((BaseAddon)this, 22000, 18, 11, 0, 667, -1, "", 1);// 93
            AddComplexComponent((BaseAddon)this, 22000, 18, 10, 0, 667, -1, "", 1);// 94
            AddComplexComponent((BaseAddon)this, 22000, 18, 9, 0, 667, -1, "", 1);// 95
            AddComplexComponent((BaseAddon)this, 22000, 18, 8, 0, 667, -1, "", 1);// 96
            AddComplexComponent((BaseAddon)this, 22000, 18, 7, 0, 667, -1, "", 1);// 97
            AddComplexComponent((BaseAddon)this, 22000, 17, 11, 0, 667, -1, "", 1);// 98
            AddComplexComponent((BaseAddon)this, 22000, 17, 10, 0, 667, -1, "", 1);// 99
            AddComplexComponent((BaseAddon)this, 22000, 17, 9, 0, 667, -1, "", 1);// 100
            AddComplexComponent((BaseAddon)this, 22000, 17, 8, 0, 667, -1, "", 1);// 101
            AddComplexComponent((BaseAddon)this, 22000, 17, 7, 0, 667, -1, "", 1);// 102
            AddComplexComponent((BaseAddon)this, 22000, 18, 6, 0, 667, -1, "", 1);// 103
            AddComplexComponent((BaseAddon)this, 22000, 19, 6, 0, 667, -1, "", 1);// 104
            AddComplexComponent((BaseAddon)this, 22000, 20, 6, 0, 667, -1, "", 1);// 105
            AddComplexComponent((BaseAddon)this, 22000, 20, 5, 0, 667, -1, "", 1);// 106
            AddComplexComponent((BaseAddon)this, 22000, 19, 5, 0, 667, -1, "", 1);// 107
            AddComplexComponent((BaseAddon)this, 22000, 18, 5, 0, 667, -1, "", 1);// 108
            AddComplexComponent((BaseAddon)this, 22000, 17, 6, 0, 667, -1, "", 1);// 109
            AddComplexComponent((BaseAddon)this, 22000, 16, 6, 0, 667, -1, "", 1);// 110
            AddComplexComponent((BaseAddon)this, 22000, 16, 7, 0, 667, -1, "", 1);// 111
            AddComplexComponent((BaseAddon)this, 22000, 16, 8, 0, 667, -1, "", 1);// 112
            AddComplexComponent((BaseAddon)this, 22000, 16, 9, 0, 667, -1, "", 1);// 113
            AddComplexComponent((BaseAddon)this, 22000, 16, 10, 0, 667, -1, "", 1);// 114
            AddComplexComponent((BaseAddon)this, 22000, 16, 11, 0, 667, -1, "", 1);// 115
            AddComplexComponent((BaseAddon)this, 22000, 15, 11, 0, 667, -1, "", 1);// 116
            AddComplexComponent((BaseAddon)this, 22000, 15, 10, 0, 667, -1, "", 1);// 117
            AddComplexComponent((BaseAddon)this, 22000, 15, 9, 0, 667, -1, "", 1);// 118
            AddComplexComponent((BaseAddon)this, 22000, 15, 8, 0, 667, -1, "", 1);// 119
            AddComplexComponent((BaseAddon)this, 22000, 15, 7, 0, 667, -1, "", 1);// 120
            AddComplexComponent((BaseAddon)this, 22000, 15, 6, 0, 667, -1, "", 1);// 121
            AddComplexComponent((BaseAddon)this, 22000, 17, 5, 0, 667, -1, "", 1);// 122
            AddComplexComponent((BaseAddon)this, 22000, 16, 5, 0, 667, -1, "", 1);// 123
            AddComplexComponent((BaseAddon)this, 22000, 15, 5, 0, 667, -1, "", 1);// 124
            AddComplexComponent((BaseAddon)this, 22000, 14, 5, 0, 667, -1, "", 1);// 125
            AddComplexComponent((BaseAddon)this, 22000, 14, 6, 0, 667, -1, "", 1);// 126
            AddComplexComponent((BaseAddon)this, 22000, 14, 7, 0, 667, -1, "", 1);// 127
            AddComplexComponent((BaseAddon)this, 22000, 14, 9, 0, 667, -1, "", 1);// 128
            AddComplexComponent((BaseAddon)this, 22000, 14, 10, 0, 667, -1, "", 1);// 129
            AddComplexComponent((BaseAddon)this, 22000, 14, 11, 0, 667, -1, "", 1);// 130
            AddComplexComponent((BaseAddon)this, 22000, 13, 11, 0, 667, -1, "", 1);// 131
            AddComplexComponent((BaseAddon)this, 22000, 13, 10, 0, 667, -1, "", 1);// 132
            AddComplexComponent((BaseAddon)this, 22000, 13, 9, 0, 667, -1, "", 1);// 133
            AddComplexComponent((BaseAddon)this, 22000, 13, 8, 0, 667, -1, "", 1);// 134
            AddComplexComponent((BaseAddon)this, 22000, 14, 8, 0, 667, -1, "", 1);// 135
            AddComplexComponent((BaseAddon)this, 22000, 13, 7, 0, 667, -1, "", 1);// 136
            AddComplexComponent((BaseAddon)this, 22000, 13, 6, 0, 667, -1, "", 1);// 137
            AddComplexComponent((BaseAddon)this, 22000, 13, 5, 0, 667, -1, "", 1);// 138
            AddComplexComponent((BaseAddon)this, 17724, 13, 11, 5, 1910, -1, "", 1);// 139
            AddComplexComponent((BaseAddon)this, 17724, 13, 10, 10, 1910, -1, "", 1);// 140
            AddComplexComponent((BaseAddon)this, 17724, 13, 9, 5, 1910, -1, "", 1);// 141
            AddComplexComponent((BaseAddon)this, 17734, 13, 8, 10, 1910, -1, "", 1);// 142
            AddComplexComponent((BaseAddon)this, 17714, 13, 7, 5, 1910, -1, "", 1);// 143
            AddComplexComponent((BaseAddon)this, 17725, 13, 6, 0, 1910, -1, "", 1);// 144
            AddComplexComponent((BaseAddon)this, 17738, 13, 11, 0, 1910, -1, "", 1);// 145
            AddComplexComponent((BaseAddon)this, 17741, 13, 10, 0, 1910, -1, "", 1);// 146
            AddComplexComponent((BaseAddon)this, 17739, 13, 9, 0, 1910, -1, "", 1);// 147
            AddComplexComponent((BaseAddon)this, 17737, 13, 8, 0, 1910, -1, "", 1);// 148
            AddComplexComponent((BaseAddon)this, 17732, 13, 7, 0, 1910, -1, "", 1);// 149
            AddComplexComponent((BaseAddon)this, 17730, 13, 6, 0, 1910, -1, "", 1);// 150
            AddComplexComponent((BaseAddon)this, 17747, 13, 5, 0, 1910, -1, "", 1);// 151
            AddComplexComponent((BaseAddon)this, 4958, -9, 12, 0, 2429, -1, "", 1);// 169
            AddComplexComponent((BaseAddon)this, 4957, -9, 13, 0, 2429, -1, "", 1);// 170
            AddComplexComponent((BaseAddon)this, 4960, -10, 9, 0, 2429, -1, "", 1);// 176
            AddComplexComponent((BaseAddon)this, 4953, -9, 9, 0, 2429, -1, "", 1);// 177
            AddComplexComponent((BaseAddon)this, 4952, -9, 11, 0, 2429, -1, "", 1);// 178
            AddComplexComponent((BaseAddon)this, 4960, -7, 5, 4, 2429, -1, "", 1);// 198
            AddComplexComponent((BaseAddon)this, 4963, -7, 5, 0, 2429, -1, "", 1);// 200
            AddComplexComponent((BaseAddon)this, 4962, -8, 5, 2, 2429, -1, "", 1);// 201
            AddComplexComponent((BaseAddon)this, 4952, -9, 5, 2, 2429, -1, "", 1);// 202
            AddComplexComponent((BaseAddon)this, 4970, -7, 5, 2, 2429, -1, "", 1);// 203
            AddComplexComponent((BaseAddon)this, 4967, -8, 5, 0, 2429, -1, "", 1);// 204
            AddComplexComponent((BaseAddon)this, 4957, -17, 8, 3, 2429, -1, "", 1);// 379
            AddComplexComponent((BaseAddon)this, 4958, -14, 9, 3, 2429, -1, "", 1);// 380
            AddComplexComponent((BaseAddon)this, 4962, -18, 0, 3, 2429, -1, "", 1);// 381
            AddComplexComponent((BaseAddon)this, 4950, -18, 3, 3, 2429, -1, "", 1);// 382
            AddComplexComponent((BaseAddon)this, 4962, -18, 7, 3, 2429, -1, "", 1);// 383
            AddComplexComponent((BaseAddon)this, 4960, -18, -2, 3, 2429, -1, "", 1);// 384
            AddComplexComponent((BaseAddon)this, 4963, -18, -4, 0, 2429, -1, "", 1);// 385
            AddComplexComponent((BaseAddon)this, 4963, -18, -3, 0, 2429, -1, "", 1);// 386
            AddComplexComponent((BaseAddon)this, 4963, -18, -2, 0, 2429, -1, "", 1);// 387
            AddComplexComponent((BaseAddon)this, 4963, -18, -1, 0, 2429, -1, "", 1);// 388
            AddComplexComponent((BaseAddon)this, 4963, -18, 0, 0, 2429, -1, "", 1);// 389
            AddComplexComponent((BaseAddon)this, 4963, -18, 1, 0, 2429, -1, "", 1);// 390
            AddComplexComponent((BaseAddon)this, 4963, -18, 2, 0, 2429, -1, "", 1);// 391
            AddComplexComponent((BaseAddon)this, 4963, -18, 4, 0, 2429, -1, "", 1);// 393
            AddComplexComponent((BaseAddon)this, 4963, -18, 5, 0, 2429, -1, "", 1);// 394
            AddComplexComponent((BaseAddon)this, 4963, -18, 6, 0, 2429, -1, "", 1);// 395
            AddComplexComponent((BaseAddon)this, 4963, -17, 7, 0, 2429, -1, "", 1);// 397
            AddComplexComponent((BaseAddon)this, 4963, -17, 8, 0, 2429, -1, "", 1);// 398
            AddComplexComponent((BaseAddon)this, 4963, -16, 8, 0, 2429, -1, "", 1);// 399
            AddComplexComponent((BaseAddon)this, 4963, -15, 8, 0, 2429, -1, "", 1);// 400
            AddComplexComponent((BaseAddon)this, 4963, -14, 9, 0, 2429, -1, "", 1);// 401
            AddComplexComponent((BaseAddon)this, 4963, -13, 9, 0, 2429, -1, "", 1);// 402
            AddComplexComponent((BaseAddon)this, 4963, -12, 9, 0, 2429, -1, "", 1);// 403
            AddComplexComponent((BaseAddon)this, 4963, -11, 9, 0, 2429, -1, "", 1);// 404
            AddComplexComponent((BaseAddon)this, 4961, -11, 9, 0, 2429, -1, "", 1);// 405
            AddComplexComponent((BaseAddon)this, 4960, -12, 3, 3, 2429, -1, "", 1);// 433
            AddComplexComponent((BaseAddon)this, 4950, -14, -2, 3, 2429, -1, "", 1);// 434
            AddComplexComponent((BaseAddon)this, 4967, -14, -1, 0, 2429, -1, "", 1);// 435
            AddComplexComponent((BaseAddon)this, 4967, -14, -3, 0, 2429, -1, "", 1);// 436
            AddComplexComponent((BaseAddon)this, 4963, -14, -4, 0, 2429, -1, "", 1);// 439
            AddComplexComponent((BaseAddon)this, 4963, -14, -2, 0, 2429, -1, "", 1);// 440
            AddComplexComponent((BaseAddon)this, 4963, -14, 0, 0, 2429, -1, "", 1);// 441
            AddComplexComponent((BaseAddon)this, 4963, -14, 1, 0, 2429, -1, "", 1);// 442
            AddComplexComponent((BaseAddon)this, 4944, -14, 2, 0, 2429, -1, "", 1);// 443
            AddComplexComponent((BaseAddon)this, 4963, -13, 2, 0, 2429, -1, "", 1);// 444
            AddComplexComponent((BaseAddon)this, 4963, -14, 3, 0, 2429, -1, "", 1);// 445
            AddComplexComponent((BaseAddon)this, 4963, -13, 3, 0, 2429, -1, "", 1);// 446
            AddComplexComponent((BaseAddon)this, 4963, -12, 3, 0, 2429, -1, "", 1);// 447
            AddComplexComponent((BaseAddon)this, 4963, -11, 3, 0, 2429, -1, "", 1);// 448
            AddComplexComponent((BaseAddon)this, 4963, -11, 3, 5, 2429, -1, "", 1);// 449
            AddComplexComponent((BaseAddon)this, 4956, -11, 3, 0, 2429, -1, "", 1);// 450
            AddComplexComponent((BaseAddon)this, 4960, -19, -14, 3, 2429, -1, "", 1);// 510
            AddComplexComponent((BaseAddon)this, 4950, -18, -5, 3, 2429, -1, "", 1);// 511
            AddComplexComponent((BaseAddon)this, 4962, -19, -20, 3, 2429, -1, "", 1);// 512
            AddComplexComponent((BaseAddon)this, 4962, -18, -12, 3, 2429, -1, "", 1);// 513
            AddComplexComponent((BaseAddon)this, 4962, -19, -17, 3, 2429, -1, "", 1);// 514
            AddComplexComponent((BaseAddon)this, 4960, -18, -8, 3, 2429, -1, "", 1);// 515
            AddComplexComponent((BaseAddon)this, 4963, -19, -20, 0, 2429, -1, "", 1);// 516
            AddComplexComponent((BaseAddon)this, 4963, -19, -19, 0, 2429, -1, "", 1);// 517
            AddComplexComponent((BaseAddon)this, 4963, -19, -18, 0, 2429, -1, "", 1);// 518
            AddComplexComponent((BaseAddon)this, 4963, -19, -17, 0, 2429, -1, "", 1);// 519
            AddComplexComponent((BaseAddon)this, 4963, -19, -16, 0, 2429, -1, "", 1);// 520
            AddComplexComponent((BaseAddon)this, 4963, -19, -15, 0, 2429, -1, "", 1);// 521
            AddComplexComponent((BaseAddon)this, 4963, -19, -14, 0, 2429, -1, "", 1);// 522
            AddComplexComponent((BaseAddon)this, 4963, -19, -13, 0, 2429, -1, "", 1);// 523
            AddComplexComponent((BaseAddon)this, 4963, -18, -12, 0, 2429, -1, "", 1);// 524
            AddComplexComponent((BaseAddon)this, 4963, -18, -11, 0, 2429, -1, "", 1);// 525
            AddComplexComponent((BaseAddon)this, 4963, -18, -10, 0, 2429, -1, "", 1);// 526
            AddComplexComponent((BaseAddon)this, 4963, -18, -9, 0, 2429, -1, "", 1);// 527
            AddComplexComponent((BaseAddon)this, 4963, -18, -8, 0, 2429, -1, "", 1);// 528
            AddComplexComponent((BaseAddon)this, 4963, -18, -7, 0, 2429, -1, "", 1);// 529
            AddComplexComponent((BaseAddon)this, 4963, -18, -6, 0, 2429, -1, "", 1);// 530
            AddComplexComponent((BaseAddon)this, 4963, -18, -5, 0, 2429, -1, "", 1);// 531
            AddComplexComponent((BaseAddon)this, 4960, -14, -16, 2, 2429, -1, "", 1);// 592
            AddComplexComponent((BaseAddon)this, 4960, -14, -15, 2, 2429, -1, "", 1);// 593
            AddComplexComponent((BaseAddon)this, 4960, -14, -10, 3, 2429, -1, "", 1);// 594
            AddComplexComponent((BaseAddon)this, 4960, -14, -19, 3, 2429, -1, "", 1);// 595
            AddComplexComponent((BaseAddon)this, 4949, -14, -13, 2, 2429, -1, "", 1);// 596
            AddComplexComponent((BaseAddon)this, 4946, -14, -18, 3, 2429, -1, "", 1);// 597
            AddComplexComponent((BaseAddon)this, 4950, -14, -6, 0, 2429, -1, "", 1);// 598
            AddComplexComponent((BaseAddon)this, 4966, -14, -20, 3, 2429, -1, "", 1);// 599
            AddComplexComponent((BaseAddon)this, 4966, -14, -17, 3, 2429, -1, "", 1);// 600
            AddComplexComponent((BaseAddon)this, 4966, -14, -12, 3, 2429, -1, "", 1);// 601
            AddComplexComponent((BaseAddon)this, 4966, -14, -11, 3, 2429, -1, "", 1);// 602
            AddComplexComponent((BaseAddon)this, 4966, -14, -8, 3, 2429, -1, "", 1);// 603
            AddComplexComponent((BaseAddon)this, 4967, -14, -9, 0, 2429, -1, "", 1);// 605
            AddComplexComponent((BaseAddon)this, 4967, -14, -7, 0, 2429, -1, "", 1);// 606
            AddComplexComponent((BaseAddon)this, 4967, -14, -14, 0, 2429, -1, "", 1);// 607
            AddComplexComponent((BaseAddon)this, 4967, -14, -13, 0, 2429, -1, "", 1);// 608
            AddComplexComponent((BaseAddon)this, 4967, -14, -15, 0, 2429, -1, "", 1);// 609
            AddComplexComponent((BaseAddon)this, 4967, -14, -16, 0, 2429, -1, "", 1);// 610
            AddComplexComponent((BaseAddon)this, 4969, -14, -16, 0, 2429, -1, "", 1);// 611
            AddComplexComponent((BaseAddon)this, 4969, -14, -15, 0, 2429, -1, "", 1);// 612
            AddComplexComponent((BaseAddon)this, 4963, -14, -20, 0, 2429, -1, "", 1);// 615
            AddComplexComponent((BaseAddon)this, 4963, -14, -19, 0, 2429, -1, "", 1);// 616
            AddComplexComponent((BaseAddon)this, 4963, -14, -18, 0, 2429, -1, "", 1);// 617
            AddComplexComponent((BaseAddon)this, 4963, -14, -17, 0, 2429, -1, "", 1);// 618
            AddComplexComponent((BaseAddon)this, 4963, -14, -12, 0, 2429, -1, "", 1);// 619
            AddComplexComponent((BaseAddon)this, 4963, -14, -11, 0, 2429, -1, "", 1);// 620
            AddComplexComponent((BaseAddon)this, 4963, -14, -10, 0, 2429, -1, "", 1);// 621
            AddComplexComponent((BaseAddon)this, 4963, -14, -8, 0, 2429, -1, "", 1);// 622
            AddComplexComponent((BaseAddon)this, 4963, -14, -5, 0, 2429, -1, "", 1);// 623
            AddComplexComponent((BaseAddon)this, 4960, -20, -24, 3, 2429, -1, "", 1);// 633
            AddComplexComponent((BaseAddon)this, 4960, -21, -24, 3, 2429, -1, "", 1);// 634
            AddComplexComponent((BaseAddon)this, 4963, -21, -25, 0, 2429, -1, "", 1);// 635
            AddComplexComponent((BaseAddon)this, 4963, -21, -24, 0, 2429, -1, "", 1);// 636
            AddComplexComponent((BaseAddon)this, 4963, -20, -24, 0, 2429, -1, "", 1);// 637
            AddComplexComponent((BaseAddon)this, 4963, -20, -23, 0, 2429, -1, "", 1);// 638
            AddComplexComponent((BaseAddon)this, 4963, -20, -22, 0, 2429, -1, "", 1);// 639
            AddComplexComponent((BaseAddon)this, 4963, -20, -21, 0, 2429, -1, "", 1);// 640
            AddComplexComponent((BaseAddon)this, 4950, -14, -21, 7, 2429, -1, "", 1);// 659
            AddComplexComponent((BaseAddon)this, 4961, -14, -21, 2, 2429, -1, "", 1);// 660
            AddComplexComponent((BaseAddon)this, 4960, -14, -24, 2, 2429, -1, "", 1);// 661
            AddComplexComponent((BaseAddon)this, 4960, -14, -25, 3, 2429, -1, "", 1);// 662
            AddComplexComponent((BaseAddon)this, 4948, -14, -22, 2, 2429, -1, "", 1);// 663
            AddComplexComponent((BaseAddon)this, 4966, -14, -23, 3, 2429, -1, "", 1);// 664
            AddComplexComponent((BaseAddon)this, 4967, -14, -21, 0, 2429, -1, "", 1);// 665
            AddComplexComponent((BaseAddon)this, 4967, -14, -24, 0, 2429, -1, "", 1);// 666
            AddComplexComponent((BaseAddon)this, 4963, -14, -25, 0, 2429, -1, "", 1);// 668
            AddComplexComponent((BaseAddon)this, 4963, -14, -23, 0, 2429, -1, "", 1);// 669
        }

        public BlackthornStep2(Serial serial)
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
