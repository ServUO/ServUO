
////////////////////////////////////////
//                                     //
//   Generated by CEO's YAAAG - Ver 2  //
// (Yet Another Arya Addon Generator)  //
//    Modified by Hammerhand for       //
//      SA & High Seas content         //
//                                     //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class TB_PvP_ArenaAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1305, 5, 12, 0}, {37, 7, 13, 0}, {1305, 8, 11, 0}// 1	2	3	
			, {1305, 8, 7, 0}, {1305, 3, 12, 0}, {38, 11, 10, 0}// 4	5	7	
			, {1305, 4, 8, 0}, {1305, 3, 9, 0}, {1305, 3, 11, 0}// 8	9	10	
			, {1305, 9, 11, 0}, {1305, 9, 9, 0}, {1305, 9, 7, 0}// 11	12	13	
			, {1305, 9, 10, 0}, {1305, 9, 8, 0}, {1305, 7, 9, 0}// 14	15	16	
			, {1305, 7, 8, 0}, {1305, 7, 11, 0}, {1305, 7, 10, 0}// 17	18	19	
			, {1305, 10, 11, 0}, {1305, 10, 7, 0}, {1305, 5, 8, 0}// 20	21	22	
			, {37, 3, 13, 0}, {1305, 3, 10, 0}, {1305, 10, 8, 0}// 23	24	25	
			, {1305, 10, 9, 0}, {1305, 10, 10, 0}, {1305, 4, 11, 0}// 26	27	28	
			, {1305, 6, 8, 0}, {1305, 8, 10, 0}, {1305, 3, 7, 0}// 29	30	31	
			, {38, 11, 12, 0}, {1305, 11, 10, 0}, {1305, 11, 9, 0}// 32	33	34	
			, {1305, 5, 9, 0}, {1305, 5, 10, 0}, {1305, 5, 11, 0}// 35	36	37	
			, {1305, 11, 7, 0}, {1305, 11, 8, 0}, {1305, 4, 9, 0}// 38	39	40	
			, {1305, 4, 10, 0}, {37, 10, 13, 0}, {1305, 6, 13, 0}// 41	42	43	
			, {38, 11, 11, 0}, {1305, 6, 12, 0}, {1305, 8, 13, 0}// 44	45	46	
			, {37, 5, 13, 0}, {1305, 6, 9, 0}, {1305, 3, 8, 0}// 47	48	49	
			, {1305, 8, 8, 0}, {1305, 5, 13, 0}, {1305, 6, 11, 0}// 50	51	52	
			, {1305, 4, 12, 0}, {1305, 11, 11, 0}, {38, 11, 9, 0}// 53	56	57	
			, {1305, 9, 12, 0}, {1305, 10, 12, 0}, {1305, 11, 13, 0}// 58	59	60	
			, {1305, 11, 12, 0}, {1305, 10, 13, 0}, {1305, 8, 9, 0}// 61	62	63	
			, {1305, 9, 13, 0}, {1305, 6, 10, 0}, {1305, 8, 12, 0}// 64	65	66	
			, {37, 4, 13, 0}, {1305, 7, 13, 0}, {36, 11, 13, 0}// 67	68	69	
			, {37, 9, 13, 0}, {37, 6, 13, 0}, {38, 11, 8, 0}// 70	71	72	
			, {1305, 7, 12, 0}, {37, 8, 13, 0}, {1305, 7, 7, 0}// 73	74	75	
			, {1305, 4, 13, 0}, {1305, 3, 13, 0}, {38, 11, 7, 0}// 76	77	78	
			, {2861, 9, -9, 5}, {2861, 8, -9, 5}, {2861, 7, -9, 5}// 79	80	81	
			, {2861, 6, -9, 5}, {2861, 5, -9, 5}, {2861, 3, -9, 5}// 82	83	84	
			, {2861, 4, -9, 5}, {2861, 9, -8, 0}, {2861, 8, -8, 0}// 85	86	87	
			, {2861, 6, -8, 0}, {2861, 7, -8, 0}, {2861, 5, -8, 0}// 88	89	90	
			, {2861, 4, -8, 0}, {2861, 3, -8, 0}, {1327, 10, -8, 0}// 91	92	93	
			, {1331, 11, -8, 0}, {725, 11, -9, 5}, {1822, 11, -9, 0}// 94	95	96	
			, {1822, 10, -9, 0}, {1822, 9, -9, 0}, {1822, 8, -9, 0}// 97	98	99	
			, {1822, 7, -9, 0}, {1822, 6, -9, 0}, {1822, 5, -9, 0}// 100	101	102	
			, {1822, 4, -9, 0}, {1822, 3, -9, 0}, {1327, 8, -8, 0}// 103	104	105	
			, {1327, 6, -8, 0}, {1327, 4, -8, 0}, {1331, 9, -8, 0}// 106	107	108	
			, {1331, 7, -8, 0}, {1331, 5, -8, 0}, {1331, 3, -8, 0}// 109	110	111	
			, {1331, 4, -7, 0}, {1331, 5, -6, 0}, {1331, 7, -6, 0}// 112	113	114	
			, {1331, 8, -7, 0}, {1331, 9, -6, 0}, {1331, 10, -7, 0}// 115	116	117	
			, {1331, 11, -6, 0}, {1331, 6, -7, 0}, {1331, 3, -6, 0}// 118	119	120	
			, {1327, 11, -7, 0}, {1327, 10, -6, 0}, {1327, 9, -7, 0}// 121	122	123	
			, {1327, 8, -6, 0}, {1327, 7, -7, 0}, {1327, 6, -6, 0}// 124	125	126	
			, {1327, 5, -7, 0}, {1327, 4, -6, 0}, {1327, 3, -7, 0}// 127	128	129	
			, {38, 11, 5, 0}, {1305, 3, 5, 0}, {37, 3, -5, 0}// 130	131	132	
			, {1305, 11, 6, 0}, {38, 11, -3, 0}, {38, 11, 6, 0}// 133	134	135	
			, {1305, 10, 5, 0}, {1305, 10, 2, 0}, {37, 11, -5, 0}// 137	138	140	
			, {37, 5, -5, 0}, {37, 4, -5, 0}, {1305, 10, -2, 0}// 141	142	143	
			, {1305, 4, 6, 0}, {1305, 11, 4, 0}, {1305, 11, 0, 0}// 144	147	148	
			, {1305, 10, 1, 0}, {1305, 10, 0, 0}, {38, 11, -2, 0}// 149	150	152	
			, {1305, 11, 1, 0}, {38, 11, 0, 0}, {38, 11, -1, 0}// 153	154	155	
			, {38, 11, 2, 0}, {38, 11, 1, 0}, {1305, 10, -1, 0}// 156	157	158	
			, {1305, 5, 0, 0}, {1305, 4, -4, 0}, {1305, 4, -3, 0}// 160	161	162	
			, {1305, 9, -1, 0}, {1305, 9, -2, 0}, {1305, 9, 4, 0}// 163	164	165	
			, {1305, 9, 6, 0}, {1305, 9, 5, 0}, {1305, 9, 0, 0}// 166	167	168	
			, {1305, 9, 1, 0}, {1305, 10, 3, 0}, {1305, 10, 4, 0}// 169	171	172	
			, {1305, 10, 6, 0}, {1305, 11, -4, 0}, {1305, 11, 2, 0}// 173	174	175	
			, {1305, 6, 0, 0}, {1305, 5, -1, 0}, {1305, 5, -2, 0}// 176	177	179	
			, {1305, 6, -1, 0}, {1305, 5, -3, 0}, {1305, 5, -4, 0}// 185	186	187	
			, {1305, 9, 2, 0}, {1305, 3, -1, 0}, {1305, 8, -2, 0}// 188	189	190	
			, {1305, 8, -1, 0}, {1305, 7, -4, 0}, {1309, 5, 4, 0}// 191	192	194	
			, {1305, 8, 6, 0}, {1305, 7, -2, 0}, {1305, 8, 0, 0}// 195	196	197	
			, {1305, 3, 0, 0}, {1305, 3, -3, 0}, {1305, 3, 1, 0}// 198	199	200	
			, {1305, 3, -2, 0}, {1305, 3, 6, 0}, {38, 11, 4, 0}// 201	202	203	
			, {37, 6, -5, 0}, {37, 10, -5, 0}, {37, 8, -5, 0}// 204	205	206	
			, {37, 7, -5, 0}, {37, 9, -5, 0}, {1305, 8, -3, 0}// 207	208	210	
			, {1305, 8, -4, 0}, {1305, 6, -2, 0}, {1305, 6, -4, 0}// 211	212	214	
			, {1305, 6, -3, 0}, {1305, 7, 0, 0}, {1305, 9, 3, 0}// 215	216	217	
			, {1305, 11, 5, 0}, {1305, 7, -1, 0}, {1305, 8, 2, 0}// 220	221	222	
			, {1305, 6, 6, 0}, {1305, 5, 6, 0}, {1305, 7, -3, 0}// 223	224	225	
			, {1305, 3, -4, 0}, {1305, 7, 6, 0}, {1305, 11, -3, 0}// 226	227	228	
			, {1305, 8, 4, 0}, {1305, 10, -4, 0}, {1305, 7, 1, 0}// 230	231	232	
			, {1305, 8, 1, 0}, {1305, 8, 3, 0}, {38, 11, 3, 0}// 233	234	235	
			, {1305, 9, -3, 0}, {1305, 4, -2, 0}, {1305, 11, -1, 0}// 237	238	239	
			, {1305, 7, 5, 0}, {38, 11, -4, 0}, {1305, 9, -4, 0}// 240	241	242	
			, {1305, 11, -2, 0}, {1305, 4, 0, 0}, {1305, 11, 3, 0}// 243	244	245	
			, {1305, 8, 5, 0}, {1305, 4, -1, 0}, {1305, 10, -3, 0}// 246	247	248	
			, {2861, 3, -10, 10}, {2861, 4, -11, 15}, {2861, 3, -11, 15}// 249	250	251	
			, {2861, 4, -10, 10}, {2861, 5, -10, 10}, {2861, 5, -11, 15}// 252	253	254	
			, {2861, 6, -10, 10}, {2861, 6, -11, 15}, {2861, 7, -11, 15}// 255	256	257	
			, {2861, 7, -10, 10}, {2861, 8, -11, 15}, {2861, 8, -10, 10}// 258	259	260	
			, {2861, 9, -10, 10}, {2861, 9, -11, 15}, {725, 11, -10, 10}// 261	262	263	
			, {725, 11, -11, 15}, {722, 10, -13, 15}, {722, 9, -13, 15}// 264	265	266	
			, {722, 8, -13, 15}, {722, 7, -13, 15}, {722, 6, -13, 15}// 267	268	269	
			, {722, 5, -13, 15}, {722, 4, -13, 15}, {722, 3, -13, 15}// 270	271	272	
			, {724, 11, -12, 15}, {1822, 11, -11, 10}, {1822, 10, -11, 10}// 273	274	275	
			, {1822, 9, -11, 10}, {1822, 8, -11, 10}, {1822, 7, -11, 10}// 276	277	278	
			, {1822, 6, -11, 10}, {1822, 5, -11, 10}, {1822, 4, -11, 10}// 279	280	281	
			, {1822, 3, -11, 10}, {1822, 11, -12, 10}, {1822, 10, -12, 10}// 282	283	284	
			, {1822, 9, -12, 10}, {1822, 8, -12, 10}, {1822, 7, -12, 10}// 285	286	287	
			, {1822, 6, -12, 10}, {1822, 5, -12, 10}, {1822, 4, -12, 10}// 288	289	290	
			, {1822, 3, -12, 10}, {1822, 11, -10, 5}, {1822, 10, -10, 5}// 291	292	293	
			, {1822, 9, -10, 5}, {1822, 8, -10, 5}, {1822, 7, -10, 5}// 294	295	296	
			, {1822, 6, -10, 5}, {1822, 5, -10, 5}, {1822, 4, -10, 5}// 297	298	299	
			, {1822, 3, -10, 5}, {1822, 11, -11, 5}, {1822, 10, -11, 5}// 300	301	302	
			, {1822, 9, -11, 5}, {1822, 8, -11, 5}, {1822, 7, -11, 5}// 303	304	305	
			, {1822, 6, -11, 5}, {1822, 5, -11, 5}, {1822, 4, -11, 5}// 306	307	308	
			, {1822, 3, -11, 5}, {1822, 11, -12, 5}, {1822, 10, -12, 5}// 309	310	311	
			, {1822, 9, -12, 5}, {1822, 8, -12, 5}, {1822, 7, -12, 5}// 312	313	314	
			, {1822, 6, -12, 5}, {1822, 5, -12, 5}, {1822, 4, -12, 5}// 315	316	317	
			, {1822, 3, -12, 5}, {1822, 11, -10, 0}, {1822, 11, -11, 0}// 318	319	320	
			, {1822, 11, -12, 0}, {1822, 10, -11, 0}, {1822, 10, -12, 0}// 321	322	323	
			, {1822, 9, -11, 0}, {1822, 9, -12, 0}, {1822, 8, -11, 0}// 324	325	326	
			, {1822, 8, -12, 0}, {1822, 7, -11, 0}, {1822, 7, -12, 0}// 327	328	329	
			, {1822, 6, -11, 0}, {1822, 6, -12, 0}, {1822, 5, -11, 0}// 330	331	332	
			, {1822, 5, -12, 0}, {1822, 4, -11, 0}, {1822, 4, -12, 0}// 333	334	335	
			, {1822, 3, -11, 0}, {1822, 3, -12, 0}, {1822, 10, -11, 0}// 336	337	338	
			, {1822, 9, -11, 0}, {1822, 8, -11, 0}, {1822, 7, -11, 0}// 339	340	341	
			, {1822, 6, -11, 0}, {1822, 5, -11, 0}, {1822, 4, -11, 0}// 342	343	344	
			, {1822, 3, -11, 0}, {1822, 10, -10, 0}, {1822, 9, -10, 0}// 345	346	347	
			, {1822, 8, -10, 0}, {1822, 7, -10, 0}, {1822, 6, -10, 0}// 348	349	350	
			, {1822, 5, -10, 0}, {1822, 4, -10, 0}, {1822, 3, -10, 0}// 351	352	353	
			, {723, -10, 9, 25}, {723, -10, 11, 25}, {723, -10, 12, 25}// 354	355	356	
			, {2860, -9, 9, 25}, {2860, -9, 11, 25}, {1305, 2, 13, 0}// 357	358	359	
			, {1822, -9, 8, 0}, {1822, -9, 12, 15}, {1305, 2, 7, 0}// 360	361	362	
			, {2860, -8, 11, 20}, {1327, -4, 10, 0}, {1822, -10, 13, 20}// 363	364	365	
			, {1822, -9, 7, 15}, {1822, -10, 12, 15}, {38, -2, 7, 0}// 366	367	368	
			, {1822, -8, 7, 5}, {1822, -10, 8, 10}, {1822, -10, 7, 10}// 369	370	371	
			, {1305, 0, 7, 0}, {1305, 0, 9, 0}, {1305, 0, 8, 0}// 372	373	374	
			, {1305, 0, 10, 0}, {1305, 0, 11, 0}, {722, -7, 13, 15}// 375	376	377	
			, {1305, -1, 7, 0}, {1822, -8, 13, 15}, {1305, 1, 11, 0}// 378	379	380	
			, {1305, -1, 11, 0}, {1822, -9, 13, 10}, {1822, -9, 12, 10}// 381	382	383	
			, {1822, -9, 10, 10}, {1822, -9, 9, 10}, {37, 0, 13, 0}// 384	385	386	
			, {1822, -9, 7, 10}, {1822, -9, 9, 5}, {1305, 1, 12, 0}// 387	388	389	
			, {1305, 1, 8, 0}, {1822, -8, 9, 5}, {722, -5, 13, 5}// 390	391	392	
			, {38, -2, 10, 0}, {1822, -9, 8, 20}, {38, -2, 13, 0}// 393	394	395	
			, {1305, 1, 13, 0}, {1822, -8, 7, 10}, {1327, -4, 12, 0}// 396	397	398	
			, {1822, -10, 11, 5}, {1822, -8, 10, 10}, {1822, -8, 13, 10}// 399	400	401	
			, {1822, -9, 11, 10}, {1822, -9, 8, 10}, {37, 2, 13, 0}// 402	403	404	
			, {1305, 1, 7, 0}, {1822, -9, 13, 20}, {1327, -3, 11, 0}// 405	406	407	
			, {1822, -9, 11, 20}, {1332, -4, 13, 0}, {1822, -10, 11, 20}// 408	409	410	
			, {1327, -3, 7, 0}, {1822, -6, 11, 0}, {1822, -6, 12, 0}// 411	412	413	
			, {1822, -9, 13, 5}, {1822, -8, 13, 5}, {1822, -6, 9, 5}// 414	415	416	
			, {1822, -7, 13, 10}, {1822, -9, 10, 20}, {1822, -9, 12, 20}// 417	418	419	
			, {1822, -9, 9, 20}, {1332, -5, 8, 0}, {1822, -6, 10, 0}// 420	421	422	
			, {1822, -10, 13, 5}, {1822, -7, 13, 5}, {1822, -10, 13, 10}// 423	424	425	
			, {1822, -10, 12, 10}, {1822, -6, 10, 5}, {1822, -6, 8, 5}// 426	427	428	
			, {1822, -6, 11, 5}, {1822, -6, 7, 5}, {1822, -6, 7, 0}// 429	430	431	
			, {1822, -6, 9, 0}, {1822, -6, 8, 0}, {1822, -9, 9, 0}// 432	433	434	
			, {1822, -8, 11, 15}, {1822, -9, 11, 15}, {1822, -9, 10, 15}// 435	436	437	
			, {1822, -10, 8, 20}, {1822, -7, 11, 10}, {1822, -9, 7, 20}// 438	439	440	
			, {1822, -5, 13, 0}, {1822, -9, 8, 5}, {1305, 1, 9, 0}// 441	442	443	
			, {1332, -5, 10, 0}, {1822, -10, 9, 20}, {1822, -10, 12, 20}// 444	445	446	
			, {1822, -10, 7, 15}, {1822, -8, 12, 15}, {1822, -7, 12, 10}// 447	448	449	
			, {1822, -10, 11, 15}, {1822, -7, 10, 5}, {1822, -7, 11, 5}// 450	451	452	
			, {2860, -8, 10, 20}, {1822, -6, 12, 5}, {1822, -8, 9, 15}// 453	454	455	
			, {1822, -8, 8, 15}, {1822, -8, 7, 15}, {1822, -7, 10, 10}// 456	457	458	
			, {1822, -7, 7, 10}, {1822, -7, 9, 10}, {1822, -7, 8, 10}// 459	460	461	
			, {1822, -10, 9, 15}, {1822, -9, 12, 5}, {1822, -9, 11, 5}// 462	463	464	
			, {1822, -7, 10, 0}, {1822, -7, 9, 5}, {1822, -7, 11, 0}// 465	466	467	
			, {1822, -9, 10, 0}, {1822, -10, 13, 15}, {1822, -9, 9, 15}// 468	469	470	
			, {1305, 1, 10, 0}, {1822, -8, 12, 5}, {1305, 2, 11, 0}// 471	472	473	
			, {722, -8, 13, 20}, {1822, -9, 8, 15}, {2860, -5, 10, 0}// 474	475	476	
			, {1822, -7, 9, 0}, {1822, -7, 8, 0}, {1822, -10, 8, 5}// 477	478	479	
			, {1822, -8, 11, 0}, {2860, -5, 9, 0}, {1822, -7, 7, 0}// 480	481	482	
			, {1822, -8, 10, 0}, {38, -2, 12, 0}, {1822, -10, 9, 5}// 483	484	485	
			, {2860, -7, 8, 15}, {1822, -10, 10, 5}, {1822, -10, 7, 5}// 486	487	488	
			, {1330, -2, 10, 0}, {1305, -1, 10, 0}, {1305, -1, 9, 0}// 489	490	491	
			, {1822, -7, 13, 0}, {1332, -4, 9, 0}, {1822, -8, 10, 15}// 492	493	494	
			, {1822, -9, 10, 5}, {1822, -9, 7, 0}, {2860, -9, 8, 25}// 495	496	497	
			, {1327, -3, 9, 0}, {1327, -3, 13, 0}, {38, -2, 9, 0}// 498	499	500	
			, {2860, -7, 11, 15}, {2860, -6, 11, 10}, {1327, -5, 9, 0}// 501	502	503	
			, {1327, -5, 7, 0}, {722, -9, 13, 25}, {2860, -9, 7, 25}// 504	505	506	
			, {2860, -8, 7, 20}, {1332, -4, 11, 0}, {1332, -2, 11, 0}// 507	508	509	
			, {722, -6, 13, 10}, {1330, -2, 8, 0}, {1327, -4, 8, 0}// 510	511	512	
			, {1330, -2, 12, 0}, {1332, -2, 13, 0}, {2860, -9, 10, 25}// 513	514	515	
			, {1332, -3, 10, 0}, {1332, -3, 8, 0}, {2860, -7, 9, 15}// 516	517	518	
			, {2860, -7, 10, 15}, {2860, -6, 8, 10}, {2860, -6, 9, 10}// 519	520	521	
			, {1332, -3, 12, 0}, {2860, -5, 11, 0}, {1822, -10, 9, 10}// 522	523	524	
			, {1822, -10, 11, 0}, {1305, 0, 13, 0}, {1305, 2, 8, 0}// 525	526	527	
			, {2860, -8, 9, 20}, {1305, 2, 12, 0}, {1822, -10, 8, 15}// 528	529	530	
			, {1822, -10, 10, 10}, {1822, -8, 8, 5}, {1305, -1, 8, 0}// 531	532	533	
			, {38, -2, 11, 0}, {1822, -10, 10, 15}, {1822, -7, 12, 5}// 534	535	536	
			, {1822, -10, 9, 0}, {1822, -10, 13, 0}, {1822, -8, 10, 5}// 537	538	539	
			, {1822, -8, 12, 10}, {1822, -6, 13, 5}, {1822, -10, 10, 0}// 540	541	542	
			, {1822, -8, 9, 10}, {1822, -8, 8, 10}, {1822, -6, 13, 0}// 543	544	545	
			, {1822, -8, 11, 10}, {1822, -7, 7, 5}, {2860, -7, 7, 15}// 546	547	548	
			, {723, -10, 10, 25}, {723, -10, 7, 25}, {723, -10, 8, 25}// 549	550	551	
			, {1822, -10, 12, 0}, {1822, -7, 12, 0}, {37, -1, 13, 0}// 552	553	554	
			, {1822, -10, 8, 0}, {1327, -5, 11, 0}, {1305, -1, 13, 0}// 555	556	557	
			, {1305, 0, 12, 0}, {1305, -1, 12, 0}, {37, 1, 13, 0}// 558	559	560	
			, {1822, -5, 12, 0}, {1822, -9, 11, 0}, {1822, -10, 10, 20}// 561	562	563	
			, {1822, -8, 11, 5}, {1822, -8, 7, 0}, {1822, -9, 7, 5}// 564	565	566	
			, {1822, -10, 12, 5}, {1332, -2, 9, 0}, {1822, -10, 11, 10}// 567	568	569	
			, {1822, -8, 8, 0}, {2860, -5, 8, 0}, {1305, 2, 9, 0}// 570	571	572	
			, {2860, -6, 10, 10}, {1822, -10, 7, 20}, {1822, -9, 13, 0}// 573	574	575	
			, {1822, -7, 8, 5}, {2860, -5, 7, 0}, {2860, -8, 8, 20}// 576	577	578	
			, {1822, -9, 12, 0}, {1822, -8, 9, 0}, {1822, -10, 7, 0}// 579	580	581	
			, {1822, -8, 13, 0}, {1822, -9, 13, 15}, {38, -2, 8, 0}// 582	583	584	
			, {1822, -8, 12, 0}, {1305, 2, 10, 0}, {2860, -6, 7, 10}// 585	586	587	
			, {1332, -4, 7, 0}, {1332, -2, 7, 0}, {721, -10, 13, 25}// 588	589	590	
			, {1822, -6, -5, 5}, {2861, -2, -9, 5}, {2861, -1, -9, 5}// 591	592	593	
			, {2861, 2, -9, 5}, {2861, 1, -9, 5}, {2861, 0, -9, 5}// 594	595	596	
			, {2861, 2, -8, 0}, {2861, 1, -8, 0}, {2861, 0, -8, 0}// 597	598	599	
			, {2861, -2, -8, 0}, {2861, -1, -8, 0}, {1327, -2, -8, 0}// 600	601	602	
			, {1327, -4, -8, 0}, {1331, -3, -8, 0}, {725, -4, -9, 5}// 603	604	605	
			, {1822, -3, -9, 0}, {1822, -2, -9, 0}, {1822, 2, -9, 0}// 606	607	608	
			, {1822, 1, -9, 0}, {1822, 0, -9, 0}, {1822, -1, -9, 0}// 609	610	611	
			, {1327, 2, -8, 0}, {1327, 0, -8, 0}, {1331, 1, -8, 0}// 612	613	614	
			, {1331, -1, -8, 0}, {1331, 2, -7, 0}, {1331, 1, -6, 0}// 615	616	617	
			, {1331, 0, -7, 0}, {1331, -1, -6, 0}, {1331, -2, -7, 0}// 618	619	620	
			, {1331, -3, -6, 0}, {1331, -4, -7, 0}, {1327, 2, -6, 0}// 621	622	623	
			, {1327, 1, -7, 0}, {1327, 0, -6, 0}, {1327, -1, -7, 0}// 624	625	626	
			, {1327, -2, -6, 0}, {1327, -3, -7, 0}, {1327, -4, -6, 0}// 627	628	629	
			, {2860, -7, 3, 15}, {2860, -7, 1, 15}, {2860, -8, -4, 20}// 630	631	632	
			, {2860, -9, -3, 25}, {723, -10, 3, 25}, {723, -10, 6, 25}// 633	634	635	
			, {2860, -7, -3, 15}, {2860, -7, -4, 15}, {723, -10, 2, 25}// 636	637	638	
			, {1822, -8, -3, 5}, {1305, 1, -2, 0}, {1332, -5, 0, 0}// 639	640	641	
			, {1305, 1, 3, 0}, {1822, -9, -4, 15}, {1305, 2, 6, 0}// 642	643	644	
			, {1822, -8, 6, 10}, {1332, -2, -1, 0}, {38, -2, -3, 0}// 645	646	647	
			, {1822, -10, -2, 20}, {1305, -1, -1, 0}, {1822, -6, 0, 0}// 648	649	650	
			, {1822, -9, -1, 20}, {1822, -7, -5, 5}, {1822, -8, -3, 15}// 651	652	653	
			, {1822, -10, -1, 5}, {2860, -5, -4, 0}, {1822, -10, -2, 5}// 654	655	656	
			, {1305, -1, -3, 0}, {1305, -1, -2, 0}, {2860, -6, 6, 10}// 657	658	659	
			, {1822, -10, 5, 10}, {1822, -10, 4, 10}, {1822, -9, 0, 10}// 660	661	662	
			, {1822, -10, 0, 0}, {1822, -9, 2, 10}, {1305, 1, -4, 0}// 663	664	665	
			, {1822, -8, 1, 15}, {1327, -3, 3, 0}, {1822, -9, 0, 20}// 666	667	668	
			, {1822, -8, 4, 5}, {1822, -10, -5, 10}, {1822, -10, 4, 20}// 669	670	671	
			, {1822, -10, 5, 20}, {1822, -9, 1, 0}, {1305, 1, 5, 0}// 672	673	674	
			, {1305, 1, 6, 0}, {1822, -10, -1, 0}, {1305, -1, 2, 0}// 675	676	677	
			, {1822, -6, -5, 0}, {1822, -8, 4, 15}, {1305, 0, -4, 0}// 678	679	680	
			, {1305, 0, 5, 0}, {1305, 0, 6, 0}, {1305, -1, 3, 0}// 681	682	683	
			, {1305, -1, 4, 0}, {1822, -9, 1, 15}, {1822, -10, -2, 0}// 684	685	686	
			, {2860, -9, -4, 25}, {1822, -8, 2, 5}, {2860, -8, 4, 20}// 687	688	689	
			, {1305, 0, -1, 0}, {1305, -1, 6, 0}, {1822, -6, -3, 0}// 690	691	692	
			, {1332, -5, 2, 0}, {1822, -7, -5, 10}, {1822, -8, -5, 10}// 693	694	695	
			, {2860, -7, 6, 15}, {1305, 2, 2, 0}, {1305, 2, -4, 0}// 696	697	698	
			, {1305, 1, 1, 0}, {1822, -10, -4, 0}, {1822, -9, -1, 10}// 699	700	701	
			, {1822, -10, -5, 20}, {1305, 1, 0, 0}, {1822, -6, 5, 0}// 702	703	704	
			, {1822, -10, 2, 10}, {1822, -7, -2, 10}, {1305, 1, -3, 0}// 705	706	707	
			, {1822, -9, -3, 15}, {1822, -5, -5, 0}, {1822, -8, 4, 10}// 708	709	710	
			, {1327, -5, -3, 0}, {37, -1, -5, 0}, {1822, -6, 1, 0}// 711	712	713	
			, {39, -2, -5, 0}, {1822, -7, 0, 0}, {38, -2, 1, 0}// 714	715	716	
			, {1822, -9, 3, 0}, {1305, 2, -3, 0}, {2860, -5, 4, 0}// 717	718	719	
			, {2860, -5, 5, 0}, {2860, -5, 6, 0}, {1822, -10, -4, 10}// 720	721	722	
			, {2860, -5, 2, 0}, {1822, -10, -1, 15}, {1822, -9, 6, 10}// 723	724	725	
			, {1822, -7, -1, 5}, {1822, -8, -1, 5}, {1822, -7, -3, 5}// 726	727	728	
			, {1822, -9, -1, 15}, {1822, -9, -5, 15}, {1822, -8, -5, 15}// 729	730	731	
			, {1822, -10, 2, 15}, {1822, -9, -3, 10}, {1822, -10, 0, 15}// 732	733	734	
			, {1822, -10, 1, 15}, {1822, -9, -2, 10}, {1822, -8, -3, 10}// 735	736	737	
			, {1822, -7, -3, 10}, {1822, -9, 3, 15}, {1822, -10, -4, 15}// 738	739	740	
			, {1822, -7, -2, 5}, {1822, -9, 0, 15}, {1822, -9, 2, 15}// 741	742	743	
			, {2860, -5, 3, 0}, {1822, -9, -4, 10}, {723, -10, -2, 25}// 744	745	746	
			, {1822, -10, 3, 15}, {1822, -10, -3, 10}, {1822, -10, -5, 15}// 747	748	749	
			, {1822, -6, 3, 5}, {1822, -6, 5, 5}, {1822, -6, 4, 5}// 750	751	752	
			, {1822, -6, 0, 5}, {1822, -10, 0, 10}, {1822, -6, 6, 5}// 753	754	755	
			, {1822, -6, 6, 0}, {1822, -6, -3, 5}, {1822, -6, -1, 5}// 756	757	758	
			, {1822, -6, -2, 5}, {1822, -10, 6, 20}, {1822, -8, 5, 5}// 759	760	761	
			, {1332, -4, 1, 0}, {1327, -4, 2, 0}, {2860, -7, 4, 15}// 762	763	764	
			, {1327, -3, -5, 0}, {1822, -9, -2, 20}, {1822, -9, -3, 20}// 765	766	767	
			, {1332, -3, 0, 0}, {1822, -8, 0, 10}, {1332, -3, -2, 0}// 768	769	770	
			, {1332, -3, -4, 0}, {1822, -9, 3, 10}, {1822, -9, 4, 10}// 771	772	773	
			, {1822, -7, 0, 10}, {1822, -6, -4, 0}, {1822, -9, 5, 10}// 774	775	776	
			, {1822, -10, 2, 20}, {2860, -8, -1, 20}, {37, 1, -5, 0}// 777	778	779	
			, {37, 0, -5, 0}, {1327, -3, 5, 0}, {2860, -8, 3, 20}// 780	781	782	
			, {1822, -6, 2, 5}, {1822, -6, 1, 5}, {1822, -9, 4, 0}// 783	784	785	
			, {1327, -5, -1, 0}, {1305, 2, 1, 0}, {1305, 2, -2, 0}// 786	787	788	
			, {1822, -10, -2, 10}, {2860, -6, 5, 10}, {2860, -8, -3, 20}// 789	790	791	
			, {1822, -10, -1, 10}, {722, -10, -6, 25}, {1822, -10, 3, 20}// 792	793	794	
			, {1332, -3, 2, 0}, {1822, -10, 5, 15}, {1822, -10, 6, 15}// 795	796	797	
			, {1822, -10, 4, 15}, {1305, 2, -1, 0}, {1305, 1, 2, 0}// 798	799	800	
			, {1822, -8, 3, 15}, {723, -10, -3, 25}, {37, 2, -5, 0}// 801	802	803	
			, {2860, -7, 0, 15}, {1822, -8, 6, 15}, {1822, -7, 2, 10}// 804	805	806	
			, {1822, -7, 4, 10}, {1822, -7, 1, 10}, {1822, -7, 5, 10}// 807	808	809	
			, {1822, -7, 6, 10}, {1822, -7, 3, 10}, {1822, -9, -5, 10}// 810	811	812	
			, {1822, -9, -4, 5}, {1822, -9, 2, 5}, {1822, -9, 1, 5}// 813	814	815	
			, {1822, -10, 3, 10}, {1822, -7, 5, 0}, {38, -2, -2, 0}// 816	817	818	
			, {1332, -2, 1, 0}, {1822, -7, 1, 5}, {1822, -7, 2, 5}// 819	820	821	
			, {1822, -7, 3, 5}, {1822, -8, 0, 5}, {1822, -7, 0, 5}// 822	823	824	
			, {1822, -7, -4, 5}, {1822, -9, 0, 5}, {1822, -7, 6, 5}// 825	826	827	
			, {1822, -10, 0, 5}, {1822, -8, -4, 5}, {1822, -7, 4, 5}// 828	829	830	
			, {2860, -6, 4, 10}, {1305, 2, 5, 0}, {1330, -2, -4, 0}// 831	832	833	
			, {1332, -2, 3, 0}, {1822, -9, 5, 5}, {1305, 0, 1, 0}// 834	835	836	
			, {1305, 0, 0, 0}, {1305, 0, 3, 0}, {1305, 0, 2, 0}// 837	838	839	
			, {1327, -4, 0, 0}, {1822, -10, 2, 0}, {1305, -1, 1, 0}// 840	841	842	
			, {1305, -1, 0, 0}, {1305, 1, 4, 0}, {1305, 2, 3, 0}// 843	844	845	
			, {1822, -8, -5, 5}, {1822, -9, 6, 0}, {1822, -9, 6, 5}// 846	847	848	
			, {1305, 1, -1, 0}, {1305, 2, 0, 0}, {1822, -9, 2, 0}// 849	850	851	
			, {1822, -8, 2, 15}, {1327, -4, 4, 0}, {1822, -10, 1, 10}// 852	853	854	
			, {2860, -6, -1, 10}, {38, -2, 3, 0}, {1822, -9, 6, 20}// 855	856	857	
			, {38, -2, 6, 0}, {38, -2, 4, 0}, {1822, -7, 4, 0}// 858	859	860	
			, {1822, -7, 1, 0}, {1822, -7, -1, 0}, {38, -2, 5, 0}// 861	862	863	
			, {1822, -7, 2, 0}, {1822, -10, 4, 5}, {1822, -7, 3, 0}// 864	865	866	
			, {1822, -10, 5, 5}, {1822, -7, 6, 0}, {1822, -10, 6, 5}// 867	868	869	
			, {1822, -8, 3, 5}, {1822, -10, -3, 0}, {1332, -4, -3, 0}// 870	871	872	
			, {1822, -9, 4, 5}, {1332, -4, 3, 0}, {38, -2, -4, 0}// 873	874	875	
			, {1822, -9, 5, 0}, {1822, -9, 3, 5}, {1822, -10, 1, 5}// 876	877	878	
			, {1822, -10, 2, 5}, {1822, -10, -3, 15}, {1822, -8, 2, 10}// 879	880	881	
			, {1822, -10, 1, 0}, {1327, -4, 6, 0}, {2860, -5, 1, 0}// 882	883	884	
			, {2860, -9, 6, 25}, {2860, -8, 5, 20}, {2860, -7, -1, 15}// 885	886	887	
			, {2860, -7, -2, 15}, {1332, -2, -3, 0}, {1332, -2, -5, 0}// 888	889	890	
			, {1330, -2, 4, 0}, {1330, -2, 2, 0}, {1330, -2, 6, 0}// 891	892	893	
			, {1327, -5, 5, 0}, {1327, -4, -4, 0}, {1327, -3, -3, 0}// 894	895	896	
			, {2860, -8, 0, 20}, {1327, -4, -2, 0}, {1332, -4, 5, 0}// 897	898	899	
			, {1332, -4, -5, 0}, {1327, -3, 1, 0}, {1332, -5, -4, 0}// 900	901	902	
			, {2860, -6, 0, 10}, {2860, -5, 0, 0}, {1332, -5, 6, 0}// 903	904	905	
			, {2860, -5, -1, 0}, {1327, -5, 1, 0}, {723, -10, 4, 25}// 906	907	908	
			, {2860, -6, 2, 10}, {723, -10, -1, 25}, {1330, -2, -2, 0}// 909	910	911	
			, {1330, -2, 0, 0}, {1327, -5, 3, 0}, {1332, -5, 4, 0}// 912	913	914	
			, {1332, -3, 4, 0}, {2860, -9, 2, 25}, {2860, -8, 2, 20}// 915	916	917	
			, {2860, -8, 1, 20}, {1332, -3, 6, 0}, {722, -6, -6, 10}// 918	919	920	
			, {722, -5, -6, 5}, {722, -7, -6, 15}, {723, -10, -5, 25}// 921	922	923	
			, {722, -8, -6, 20}, {2860, -9, 5, 25}, {2860, -9, 0, 25}// 924	925	926	
			, {2860, -8, -2, 20}, {2860, -9, -1, 25}, {2860, -9, 1, 25}// 927	928	929	
			, {2860, -9, 4, 25}, {723, -10, 0, 25}, {2860, -9, -2, 25}// 930	931	932	
			, {2860, -8, 6, 20}, {2860, -9, 3, 25}, {1822, -10, -4, 20}// 933	934	935	
			, {1822, -9, 5, 15}, {1305, -1, -4, 0}, {2860, -6, 1, 10}// 936	937	938	
			, {1822, -7, -1, 10}, {1305, 0, -2, 0}, {1822, -8, -1, 10}// 939	940	941	
			, {1822, -9, -5, 20}, {1822, -9, 4, 15}, {1822, -7, -4, 5}// 942	943	944	
			, {1327, -3, -1, 0}, {1822, -8, -1, 15}, {1822, -8, 0, 0}// 945	946	947	
			, {1822, -8, -2, 15}, {1822, -8, 6, 5}, {1822, -8, -2, 10}// 948	949	950	
			, {1332, -5, -2, 0}, {722, -9, -6, 25}, {1822, -9, 4, 20}// 951	952	953	
			, {1822, -9, -5, 5}, {2860, -6, -4, 10}, {2860, -6, -3, 10}// 954	955	956	
			, {2860, -6, -2, 10}, {1822, -9, 2, 20}, {1822, -9, 3, 20}// 957	958	959	
			, {1822, -6, 2, 0}, {1822, -8, 1, 5}, {1822, -6, -4, 5}// 960	961	962	
			, {1822, -10, 0, 20}, {723, -10, 1, 25}, {2860, -6, 3, 10}// 963	964	965	
			, {1822, -6, -2, 0}, {2860, -7, 5, 15}, {723, -10, 5, 25}// 966	967	968	
			, {1822, -8, 5, 15}, {1822, -8, 3, 0}, {1822, -8, -2, 5}// 969	970	971	
			, {1822, -9, -2, 0}, {1822, -8, -4, 0}, {2860, -7, 2, 15}// 972	973	974	
			, {1822, -10, -1, 20}, {1822, -10, 3, 0}, {1822, -9, -2, 5}// 975	976	977	
			, {1822, -10, 5, 0}, {1822, -10, -5, 5}, {1822, -9, -4, 20}// 978	979	980	
			, {1822, -8, 0, 15}, {1822, -9, -5, 0}, {1822, -10, 6, 0}// 981	982	983	
			, {1822, -10, -3, 20}, {1822, -9, 0, 0}, {1822, -6, -1, 0}// 984	985	986	
			, {38, -2, 2, 0}, {1822, -8, 4, 0}, {1822, -10, 4, 0}// 987	988	989	
			, {1822, -8, 1, 10}, {1332, -2, 5, 0}, {1822, -8, 5, 0}// 990	991	992	
			, {1822, -9, -1, 0}, {1822, -8, -4, 10}, {1822, -6, 3, 0}// 993	994	995	
			, {1332, -4, -1, 0}, {1822, -10, -4, 5}, {1822, -9, -3, 5}// 996	997	998	
			, {1822, -8, 6, 0}, {1305, 0, -3, 0}, {1822, -9, 5, 20}// 999	1000	1001	
			, {1822, -10, 1, 20}, {1822, -9, -3, 0}, {1822, -7, -4, 0}// 1002	1003	1004	
			, {1822, -10, 6, 10}, {1822, -9, 6, 15}, {1822, -7, -4, 10}// 1005	1006	1007	
			, {1822, -7, 5, 5}, {1822, -10, -3, 5}, {1822, -7, -3, 0}// 1008	1009	1010	
			, {1822, -7, -2, 0}, {1822, -9, -4, 0}, {1822, -8, 3, 10}// 1011	1012	1013	
			, {1822, -9, 1, 20}, {1822, -8, -4, 15}, {38, -2, -1, 0}// 1014	1015	1016	
			, {1822, -7, -5, 0}, {1822, -8, -2, 0}, {1305, 2, 4, 0}// 1017	1018	1019	
			, {1822, -6, 4, 0}, {1822, -9, -1, 5}, {1822, -8, 2, 0}// 1020	1021	1022	
			, {1822, -9, 1, 10}, {1822, -10, -2, 15}, {1305, -1, 5, 0}// 1023	1024	1025	
			, {2860, -5, -2, 0}, {1822, -8, -1, 0}, {1305, 0, 4, 0}// 1026	1027	1028	
			, {1822, -10, 3, 5}, {1822, -8, 5, 10}, {1822, -8, 1, 0}// 1029	1030	1031	
			, {38, -2, 0, 0}, {723, -10, -4, 25}, {1822, -8, -3, 0}// 1032	1033	1034	
			, {2860, -5, -3, 0}, {1822, -9, -2, 15}, {2861, 0, -11, 15}// 1035	1036	1037	
			, {2861, -1, -11, 15}, {2861, -2, -10, 10}, {2861, 0, -10, 10}// 1038	1039	1040	
			, {2861, -1, -10, 10}, {2861, -2, -11, 15}, {2861, 1, -10, 10}// 1041	1042	1043	
			, {2861, 1, -11, 15}, {2861, 2, -10, 10}, {2861, 2, -11, 15}// 1044	1045	1046	
			, {722, 2, -13, 15}, {722, 1, -13, 15}, {1822, 1, -12, 10}// 1047	1048	1049	
			, {722, 0, -13, 15}, {722, -1, -13, 15}, {722, -2, -13, 15}// 1050	1051	1052	
			, {722, -3, -13, 15}, {725, -4, -10, 10}, {725, -4, -11, 15}// 1053	1054	1055	
			, {725, -4, -12, 15}, {1822, 2, -11, 10}, {1822, 2, -12, 10}// 1056	1057	1058	
			, {1822, 0, -12, 10}, {1822, -1, -12, 10}, {1822, 1, -11, 10}// 1059	1060	1061	
			, {1822, 0, -11, 10}, {1822, -1, -11, 10}, {1822, -2, -12, 10}// 1062	1063	1064	
			, {1822, -2, -11, 10}, {1822, -3, -12, 10}, {1822, -3, -11, 10}// 1065	1066	1067	
			, {1822, 2, -10, 5}, {1822, 1, -10, 5}, {1822, 0, -10, 5}// 1068	1069	1070	
			, {1822, -1, -10, 5}, {1822, -2, -10, 5}, {1822, -3, -10, 5}// 1071	1072	1073	
			, {1822, 2, -11, 5}, {1822, 1, -11, 5}, {1822, 0, -11, 5}// 1074	1075	1076	
			, {1822, -1, -11, 5}, {1822, -2, -11, 5}, {1822, -3, -11, 5}// 1077	1078	1079	
			, {1822, 2, -12, 5}, {1822, 1, -12, 5}, {1822, 0, -12, 5}// 1080	1081	1082	
			, {1822, -1, -12, 5}, {1822, -3, -12, 5}, {1822, -3, -10, 0}// 1083	1084	1085	
			, {1822, -3, -11, 0}, {1822, -3, -12, 0}, {1822, 2, -11, 0}// 1086	1087	1088	
			, {1822, 2, -12, 0}, {1822, 1, -11, 0}, {1822, 1, -12, 0}// 1089	1090	1091	
			, {1822, 0, -11, 0}, {1822, 0, -12, 0}, {1822, -1, -11, 0}// 1092	1093	1094	
			, {1822, -1, -12, 0}, {1822, 2, -11, 0}, {1822, 1, -11, 0}// 1095	1096	1097	
			, {1822, 0, -11, 0}, {1822, -1, -11, 0}, {1822, -2, -11, 0}// 1098	1099	1100	
			, {1822, 2, -10, 0}, {1822, 1, -10, 0}, {1822, 0, -10, 0}// 1101	1102	1103	
			, {1822, -1, -10, 0}, {1822, -2, -10, 0}// 1104	1105	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new TB_PvP_ArenaAddonDeed();
			}
		}

		[ Constructable ]
		public TB_PvP_ArenaAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 1315, 6, 7, 0, 1, -1, "", 1);// 6
			AddComplexComponent( (BaseAddon) this, 1315, 4, 7, 0, 1, -1, "", 1);// 54
			AddComplexComponent( (BaseAddon) this, 1315, 5, 7, 0, 1, -1, "", 1);// 55
			AddComplexComponent( (BaseAddon) this, 1315, 3, 4, 0, 1, -1, "", 1);// 136
			AddComplexComponent( (BaseAddon) this, 1315, 5, 5, 0, 1, -1, "", 1);// 139
			AddComplexComponent( (BaseAddon) this, 1305, 4, 3, 0, 33, -1, "", 1);// 145
			AddComplexComponent( (BaseAddon) this, 1315, 5, 1, 0, 1, -1, "", 1);// 146
			AddComplexComponent( (BaseAddon) this, 1315, 4, 1, 0, 1, -1, "", 1);// 151
			AddComplexComponent( (BaseAddon) this, 1315, 6, 1, 0, 1, -1, "", 1);// 159
			AddComplexComponent( (BaseAddon) this, 1305, 6, 3, 0, 33, -1, "", 1);// 170
			AddComplexComponent( (BaseAddon) this, 1315, 6, 2, 0, 1, -1, "", 1);// 178
			AddComplexComponent( (BaseAddon) this, 1315, 5, 2, 0, 1, -1, "", 1);// 180
			AddComplexComponent( (BaseAddon) this, 1315, 7, 2, 0, 1, -1, "", 1);// 181
			AddComplexComponent( (BaseAddon) this, 1315, 3, 3, 0, 1, -1, "", 1);// 182
			AddComplexComponent( (BaseAddon) this, 1315, 7, 4, 0, 1, -1, "", 1);// 183
			AddComplexComponent( (BaseAddon) this, 1315, 5, 3, 0, 1, -1, "", 1);// 184
			AddComplexComponent( (BaseAddon) this, 1315, 4, 5, 0, 1, -1, "", 1);// 193
			AddComplexComponent( (BaseAddon) this, 1315, 7, 3, 0, 1, -1, "", 1);// 209
			AddComplexComponent( (BaseAddon) this, 1315, 4, 4, 0, 1, -1, "", 1);// 213
			AddComplexComponent( (BaseAddon) this, 1315, 6, 4, 0, 1, -1, "", 1);// 218
			AddComplexComponent( (BaseAddon) this, 1315, 3, 2, 0, 1, -1, "", 1);// 219
			AddComplexComponent( (BaseAddon) this, 1315, 4, 2, 0, 1, -1, "", 1);// 229
			AddComplexComponent( (BaseAddon) this, 1315, 6, 5, 0, 1, -1, "", 1);// 236

		}

		public TB_PvP_ArenaAddon( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null && name.Length > 0)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType) lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class TB_PvP_ArenaAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new TB_PvP_ArenaAddon();
			}
		}

		[Constructable]
		public TB_PvP_ArenaAddonDeed()
		{
			Name = "TB_PvP_Arena";
		}

		public TB_PvP_ArenaAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}