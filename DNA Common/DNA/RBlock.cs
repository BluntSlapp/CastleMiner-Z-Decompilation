using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using DNA.Security;
using Microsoft.Xna.Framework.GamerServices;

namespace DNA
{
	public class RBlock
	{
		private DNAGame _game;

		private DateTime _releaseDate;

		public static byte[] KeyData = new byte[32]
		{
			236, 34, 252, 119, 2, 225, 246, 242, 214, 172,
			157, 191, 175, 246, 57, 246, 219, 180, 178, 196,
			212, 135, 153, 18, 146, 132, 30, 41, 238, 149,
			142, 228
		};

		private static string[] Cultures = new string[11]
		{
			"en", "es", "fr", "de", "it", "pt", "ru", "pl", "ja", "ko",
			"zh"
		};

		private byte[][] checkStr = new byte[3][]
		{
			new byte[16]
			{
				7, 64, 189, 208, 144, 16, 75, 248, 189, 255,
				162, 142, 246, 86, 250, 73
			},
			new byte[16]
			{
				135, 219, 230, 209, 206, 164, 158, 221, 141, 194,
				162, 126, 53, 20, 53, 113
			},
			new byte[16]
			{
				129, 197, 206, 48, 248, 34, 230, 176, 107, 123,
				175, 103, 173, 83, 221, 203
			}
		};

		private static byte[][] Text1 = new byte[11][]
		{
			new byte[176]
			{
				226, 63, 125, 127, 87, 124, 40, 157, 3, 27,
				8, 161, 91, 80, 88, 224, 9, 211, 134, 61,
				133, 150, 161, 143, 111, 77, 201, 11, 195, 230,
				247, 46, 93, 161, 190, 205, 169, 152, 88, 10,
				154, 104, 22, 150, 215, 90, 3, 58, 97, 117,
				235, 160, 153, 234, 102, 91, 183, 252, 177, 146,
				165, 61, 81, 13, 17, 172, 208, 73, 122, 126,
				202, 33, 34, 202, 136, 131, 164, 232, 155, 140,
				188, 183, 194, 212, 3, 118, 177, 139, 177, 78,
				29, 179, 37, 124, 4, 221, 236, 158, 83, 246,
				143, 100, 121, 39, 170, 70, 22, 25, 196, 101,
				51, 128, 227, 53, 152, 140, 141, 209, 205, 5,
				127, 158, 137, 117, 143, 253, 140, 112, 189, 59,
				164, 9, 124, 149, 1, 183, 223, 144, 37, 158,
				167, 13, 94, 69, 202, 172, 193, 40, 72, 187,
				180, 80, 152, 179, 81, 207, 224, 203, 88, 73,
				34, 47, 60, 205, 56, 238, 139, 249, 82, 60,
				166, 213, 220, 192, 129, 14
			},
			new byte[176]
			{
				40, 175, 38, 73, 221, 105, 126, 208, 244, 182,
				58, 170, 93, 72, 120, 44, 18, 125, 247, 224,
				129, 132, 234, 3, 119, 108, 224, 238, 232, 225,
				220, 17, 46, 233, 140, 246, 63, 254, 121, 41,
				211, 110, 0, 181, 133, 30, 215, 20, 62, 245,
				211, 39, 104, 98, 92, 96, 229, 190, 16, 78,
				173, 247, 98, 191, 214, 198, 230, 94, 203, 185,
				58, 145, 226, 56, 3, 38, 57, 216, 59, 115,
				63, 21, 54, 245, 126, 43, 248, 147, 34, 230,
				142, 117, 143, 157, 118, 58, 102, 155, 246, 38,
				82, 229, 115, 51, 103, 194, 37, 93, 72, 1,
				101, 34, 161, 199, 74, 111, 34, 93, 88, 137,
				252, 236, 10, 42, 93, 196, 232, 47, 15, 68,
				72, 217, 223, 223, 105, 163, 166, 107, 2, 202,
				125, 115, 75, 183, 141, 88, 168, 247, 7, 93,
				12, 61, 91, 167, 233, 129, 34, 96, 115, 149,
				118, 6, 39, 17, 23, 46, 222, 234, 81, 252,
				46, 6, 186, 38, 122, 63
			},
			new byte[176]
			{
				155, 251, 206, 179, 139, 67, 0, 99, 75, 240,
				68, 244, 233, 181, 203, 110, 218, 59, 252, 177,
				172, 40, 159, 52, 52, 163, 147, 194, 120, 6,
				234, 223, 120, 27, 95, 62, 133, 92, 58, 9,
				13, 44, 80, 251, 70, 53, 164, 150, 188, 46,
				187, 112, 69, 183, 232, 175, 80, 22, 91, 158,
				234, 74, 144, 161, 111, 168, 209, 67, 117, 242,
				156, 55, 0, 168, 231, 70, 120, 236, 69, 37,
				3, 61, 213, 37, 159, 5, 123, 144, 64, 137,
				244, 178, 237, 102, 163, 218, 66, 229, 155, 7,
				108, 158, 145, 129, 170, 188, 27, 92, 4, 45,
				211, 102, 43, 117, 210, 171, 95, 127, 140, 54,
				54, 9, 240, 121, 221, 44, 100, 138, 221, 235,
				245, 176, 94, 187, 61, 172, 114, 115, 227, 28,
				111, 34, 113, 150, 203, 108, 252, 245, 88, 15,
				143, 23, 51, 70, 211, 103, 53, 7, 82, 30,
				16, 68, 246, 102, 16, 8, 80, 84, 191, 88,
				172, 133, 126, 254, 100, 134
			},
			new byte[208]
			{
				138, 198, 93, 21, 26, 239, 158, 150, 39, 92,
				117, 188, 9, 245, 176, 49, 15, 20, 59, 201,
				56, 75, 38, 116, 24, 139, 109, 146, 112, 53,
				70, 45, 116, 222, 182, 228, 50, 40, 183, 192,
				237, 225, 11, 71, 73, 99, 99, 59, 41, 167,
				211, 12, 165, 21, 255, 141, 87, 141, 144, 6,
				27, 23, 198, 3, 240, 83, 84, 70, 229, 48,
				180, 220, 139, 179, 194, 52, 71, 81, 170, 188,
				190, 203, 49, 244, 89, 219, 155, 27, 34, 224,
				117, 147, 67, 14, 216, 139, 235, 119, 140, 148,
				139, 232, 217, 255, 4, 61, 245, 146, 107, 121,
				99, 24, 86, 55, 244, 207, 44, 18, 100, 211,
				42, 42, 18, 110, 76, 98, 213, 119, 213, 26,
				129, 11, 241, 46, 73, 4, 148, 90, 219, 192,
				200, 231, 101, 28, 125, 115, 124, 255, 207, 15,
				40, 219, 149, 156, 64, 224, 231, 75, 214, 74,
				204, 143, 133, 40, 246, 66, 155, 70, 188, 251,
				10, 119, 50, 139, 76, 46, 115, 143, 184, 255,
				105, 177, 125, 39, 80, 200, 227, 22, 255, 93,
				15, 113, 199, 200, 66, 151, 79, 164, 90, 110,
				228, 99, 199, 232, 212, 16, 28, 130
			},
			new byte[176]
			{
				21, 219, 250, 0, 242, 42, 126, 198, 172, 73,
				109, 168, 143, 80, 204, 71, 19, 214, 139, 122,
				225, 41, 215, 135, 127, 228, 80, 98, 153, 228,
				101, 176, 151, 112, 48, 87, 252, 174, 207, 99,
				46, 22, 27, 252, 223, 55, 74, 123, 36, 4,
				161, 180, 238, 20, 112, 169, 169, 97, 88, 217,
				152, 1, 248, 157, 159, 8, 58, 213, 122, 180,
				204, 127, 132, 192, 162, 12, 37, 139, 65, 65,
				115, 143, 184, 255, 105, 177, 125, 39, 80, 200,
				227, 22, 255, 93, 15, 113, 51, 238, 32, 21,
				135, 250, 17, 195, 228, 173, 22, 92, 230, 205,
				5, 138, 56, 228, 26, 236, 144, 43, 2, 112,
				24, 175, 203, 170, 114, 11, 28, 47, 0, 6,
				200, 64, 102, 8, 76, 59, 207, 203, 141, 202,
				159, 247, 241, 65, 23, 227, 194, 41, 208, 83,
				64, 112, 128, 135, 114, 215, 116, 20, 27, 254,
				160, 13, 151, 204, 66, 139, 105, 141, 202, 202,
				29, 162, 138, 153, 158, 174
			},
			new byte[192]
			{
				83, 22, 155, 42, 74, 27, 142, 134, 100, 215,
				142, 174, 67, 202, 100, 114, 161, 66, 188, 0,
				33, 248, 118, 158, 37, 249, 52, 168, 133, 96,
				90, 72, 90, 77, 78, 15, 237, 165, 252, 99,
				50, 241, 250, 71, 37, 53, 181, 204, 156, 42,
				64, 26, 41, 92, 189, 155, 9, 94, 122, 76,
				17, 229, 254, 165, 91, 173, 118, 238, 161, 156,
				62, 107, 223, 25, 25, 154, 78, 80, 27, 127,
				71, 239, 146, 92, 160, 195, 252, 116, 221, 252,
				140, 187, 125, 185, 165, 63, 156, 104, 10, 111,
				70, 126, 8, 0, 56, 209, 179, 139, 76, 213,
				38, 75, 44, 194, 113, 159, 21, 167, 13, 101,
				176, 170, 94, 58, 85, 122, 161, 131, 58, 163,
				150, 181, 249, 205, 160, 239, 146, 34, 90, 55,
				213, 219, 234, 199, 32, 89, 201, 113, 15, 86,
				32, 32, 87, 238, 134, 4, 107, 159, 231, 191,
				92, 4, 18, 235, 199, 94, 254, 14, 54, 101,
				120, 139, 35, 147, 232, 7, 252, 179, 104, 155,
				241, 18, 201, 145, 126, 59, 173, 65, 237, 237,
				137, 170
			},
			new byte[352]
			{
				32, 197, 240, 74, 22, 138, 59, 108, 134, 27,
				130, 159, 88, 152, 121, 124, 232, 222, 220, 244,
				226, 178, 159, 199, 178, 182, 8, 126, 5, 128,
				145, 106, 180, 160, 93, 134, 49, 136, 116, 92,
				90, 67, 20, 72, 45, 67, 221, 251, 178, 170,
				94, 231, 28, 59, 46, 134, 215, 161, 19, 111,
				73, 213, 172, 93, 240, 170, 130, 42, 29, 217,
				159, 83, 118, 203, 13, 122, 33, 92, 83, 91,
				91, 118, 210, 148, 252, 0, 20, 183, 65, 41,
				18, 62, 37, 69, 28, 241, 129, 201, 49, 245,
				88, 247, 131, 183, 122, 182, 53, 65, 31, 190,
				178, 90, 239, 163, 144, 168, 206, 7, 49, 167,
				253, 51, 4, 210, 4, 235, 161, 119, 100, 98,
				241, 163, 168, 211, 124, 112, 173, 99, 31, 119,
				210, 193, 227, 136, 255, 92, 79, 46, 29, 214,
				151, 0, 142, 151, 116, 155, 57, 247, 83, 74,
				6, 120, 147, 217, 175, 235, 31, 89, 111, 230,
				96, 153, 219, 32, 61, 69, 76, 130, 35, 160,
				16, 157, 66, 202, 171, 32, 52, 0, 80, 125,
				69, 21, 250, 174, 244, 79, 91, 105, 195, 210,
				103, 233, 103, 121, 66, 58, 174, 213, 93, 233,
				241, 94, 246, 163, 128, 73, 249, 184, 119, 196,
				163, 61, 241, 218, 234, 51, 47, 240, 73, 1,
				79, 147, 68, 181, 68, 208, 94, 80, 151, 177,
				211, 204, 191, 145, 10, 238, 142, 42, 35, 148,
				239, 68, 84, 252, 80, 102, 193, 49, 25, 127,
				14, 193, 250, 238, 100, 71, 216, 187, 203, 69,
				90, 223, 98, 147, 25, 93, 216, 105, 182, 35,
				169, 125, 251, 206, 191, 212, 17, 121, 251, 94,
				94, 158, 118, 130, 132, 159, 114, 146, 123, 68,
				227, 248, 173, 120, 46, 177, 3, 219, 44, 60,
				75, 153, 83, 116, 109, 125, 56, 154, 50, 85,
				173, 39, 87, 136, 202, 186, 129, 195, 168, 123,
				198, 240, 13, 84, 101, 66, 85, 165, 78, 255,
				26, 112, 21, 45, 34, 146, 222, 104, 171, 162,
				175, 83
			},
			new byte[224]
			{
				232, 134, 224, 25, 69, 147, 188, 182, 138, 64,
				205, 26, 180, 173, 141, 203, 14, 35, 36, 213,
				148, 52, 55, 241, 29, 109, 11, 14, 224, 121,
				215, 173, 183, 232, 228, 189, 217, 80, 95, 173,
				188, 147, 153, 61, 160, 51, 12, 34, 80, 3,
				146, 50, 137, 32, 132, 74, 222, 235, 114, 234,
				211, 196, 162, 210, 0, 51, 206, 225, 125, 43,
				9, 39, 249, 159, 163, 112, 181, 27, 243, 208,
				203, 39, 35, 148, 227, 97, 35, 171, 248, 253,
				223, 107, 192, 245, 127, 183, 88, 220, 205, 83,
				91, 2, 150, 46, 204, 53, 117, 135, 6, 88,
				8, 43, 156, 168, 73, 211, 113, 104, 144, 215,
				184, 6, 173, 126, 6, 4, 192, 255, 159, 11,
				56, 107, 146, 63, 134, 11, 22, 191, 24, 146,
				22, 185, 100, 6, 57, 0, 209, 5, 181, 255,
				25, 160, 72, 219, 39, 213, 223, 103, 192, 177,
				149, 127, 57, 131, 73, 254, 115, 10, 171, 236,
				129, 94, 179, 217, 225, 53, 186, 147, 11, 199,
				36, 247, 52, 177, 59, 33, 224, 101, 49, 173,
				9, 0, 141, 88, 168, 247, 7, 93, 12, 61,
				91, 167, 233, 129, 34, 96, 115, 149, 88, 250,
				18, 82, 102, 29, 38, 194, 156, 184, 105, 139,
				106, 232, 161, 179
			},
			new byte[208]
			{
				44, 62, 66, 192, 47, 21, 99, 235, 163, 216,
				52, 145, 34, 142, 138, 93, 223, 201, 121, 226,
				119, 149, 180, 250, 21, 140, 36, 136, 84, 50,
				147, 109, 237, 100, 243, 163, 182, 21, 188, 15,
				179, 203, 40, 184, 35, 43, 103, 204, 129, 198,
				95, 89, 43, 217, 246, 235, 215, 239, 192, 93,
				6, 222, 80, 95, 65, 231, 221, 177, 237, 115,
				44, 20, 10, 187, 251, 56, 242, 71, 62, 234,
				31, 55, 198, 219, 46, 89, 231, 252, 135, 214,
				107, 195, 104, 62, 223, 3, 54, 248, 74, 52,
				140, 51, 250, 21, 222, 89, 50, 104, 171, 224,
				6, 10, 9, 14, 240, 198, 14, 43, 211, 90,
				199, 157, 7, 78, 140, 196, 150, 63, 46, 3,
				147, 203, 7, 160, 28, 61, 248, 94, 182, 27,
				58, 175, 114, 149, 245, 211, 15, 106, 180, 112,
				119, 155, 210, 104, 113, 229, 91, 52, 253, 212,
				220, 5, 120, 242, 184, 195, 94, 46, 87, 101,
				184, 52, 244, 120, 106, 189, 74, 192, 86, 75,
				101, 162, 24, 105, 236, 223, 10, 247, 92, 203,
				207, 233, 57, 96, 129, 6, 69, 22, 89, 86,
				242, 168, 115, 47, 220, 10, 192, 113
			},
			new byte[208]
			{
				9, 18, 158, 40, 89, 49, 242, 248, 109, 83,
				180, 250, 90, 76, 73, 216, 234, 176, 238, 193,
				91, 80, 192, 43, 229, 75, 116, 195, 136, 205,
				219, 204, 161, 78, 93, 18, 99, 8, 222, 123,
				75, 54, 101, 165, 4, 155, 83, 141, 113, 49,
				154, 149, 91, 230, 100, 215, 104, 129, 253, 117,
				172, 232, 53, 211, 81, 6, 139, 214, 173, 35,
				123, 50, 248, 215, 246, 101, 235, 144, 17, 123,
				252, 37, 102, 238, 233, 117, 113, 250, 2, 208,
				191, 49, 197, 118, 113, 49, 123, 252, 81, 204,
				208, 184, 165, 40, 137, 24, 160, 97, 96, 214,
				249, 199, 200, 96, 112, 222, 181, 184, 123, 111,
				196, 54, 127, 162, 80, 239, 89, 10, 11, 210,
				83, 126, 181, 61, 11, 57, 255, 139, 86, 179,
				246, 34, 183, 165, 42, 191, 179, 4, 130, 252,
				112, 195, 225, 36, 19, 177, 163, 109, 44, 9,
				234, 68, 56, 26, 206, 254, 119, 228, 183, 90,
				14, 109, 199, 197, 92, 152, 221, 129, 113, 79,
				223, 70, 246, 221, 119, 42, 32, 194, 116, 81,
				133, 19, 181, 209, 119, 235, 166, 119, 56, 247,
				94, 7, 210, 149, 176, 80, 103, 246
			},
			new byte[176]
			{
				213, 90, 237, 1, 142, 199, 9, 137, 226, 56,
				104, 211, 164, 152, 11, 142, 114, 74, 167, 78,
				254, 202, 35, 207, 33, 228, 15, 132, 187, 189,
				39, 11, 136, 249, 61, 183, 249, 2, 162, 125,
				91, 79, 108, 3, 168, 214, 62, 246, 113, 138,
				31, 104, 75, 163, 93, 76, 112, 64, 46, 223,
				130, 85, 168, 42, 141, 88, 168, 247, 7, 93,
				12, 61, 91, 167, 233, 129, 34, 96, 115, 149,
				222, 135, 192, 192, 7, 138, 166, 113, 83, 123,
				24, 128, 117, 203, 250, 162, 126, 152, 41, 191,
				30, 122, 210, 0, 18, 140, 102, 185, 38, 208,
				104, 148, 108, 212, 17, 248, 125, 157, 162, 250,
				97, 133, 1, 164, 20, 12, 202, 49, 151, 20,
				191, 118, 119, 141, 117, 212, 32, 36, 98, 74,
				200, 76, 250, 88, 141, 31, 136, 197, 26, 109,
				88, 181, 244, 35, 212, 39, 133, 109, 67, 180,
				59, 231, 136, 89, 75, 187, 49, 221, 241, 253,
				33, 213, 220, 71, 60, 31
			}
		};

		private static byte[][] Text2 = new byte[11][]
		{
			new byte[16]
			{
				124, 110, 202, 216, 232, 43, 14, 228, 117, 48,
				20, 241, 193, 53, 139, 233
			},
			new byte[16]
			{
				10, 118, 155, 86, 72, 31, 42, 226, 95, 159,
				123, 116, 241, 143, 82, 155
			},
			new byte[16]
			{
				124, 110, 202, 216, 232, 43, 14, 228, 117, 48,
				20, 241, 193, 53, 139, 233
			},
			new byte[16]
			{
				124, 110, 202, 216, 232, 43, 14, 228, 117, 48,
				20, 241, 193, 53, 139, 233
			},
			new byte[16]
			{
				115, 84, 53, 2, 207, 6, 26, 102, 47, 36,
				157, 127, 63, 99, 130, 184
			},
			new byte[16]
			{
				10, 118, 155, 86, 72, 31, 42, 226, 95, 159,
				123, 116, 241, 143, 82, 155
			},
			new byte[16]
			{
				172, 10, 89, 94, 123, 2, 42, 177, 95, 19,
				94, 10, 221, 227, 250, 180
			},
			new byte[16]
			{
				73, 144, 209, 252, 22, 225, 165, 207, 235, 136,
				114, 198, 223, 253, 112, 64
			},
			new byte[16]
			{
				112, 101, 95, 67, 80, 183, 163, 93, 16, 19,
				155, 223, 129, 30, 33, 61
			},
			new byte[16]
			{
				228, 66, 19, 193, 200, 15, 31, 225, 140, 56,
				209, 188, 242, 19, 201, 116
			},
			new byte[16]
			{
				145, 122, 66, 58, 72, 246, 77, 157, 169, 13,
				224, 19, 40, 75, 78, 40
			}
		};

		private static byte[][] Text3 = new byte[11][]
		{
			new byte[32]
			{
				54, 225, 150, 233, 156, 85, 77, 246, 38, 9,
				30, 91, 167, 211, 175, 78, 49, 46, 64, 154,
				221, 227, 114, 94, 92, 222, 142, 54, 247, 242,
				19, 87
			},
			new byte[32]
			{
				181, 70, 44, 3, 47, 24, 195, 38, 218, 98,
				221, 194, 141, 241, 211, 104, 145, 68, 85, 160,
				32, 133, 46, 98, 230, 178, 55, 203, 57, 254,
				195, 111
			},
			new byte[32]
			{
				187, 236, 65, 212, 201, 16, 147, 41, 6, 35,
				6, 118, 150, 176, 169, 104, 235, 199, 147, 215,
				196, 247, 127, 186, 59, 96, 23, 30, 66, 87,
				25, 35
			},
			new byte[32]
			{
				161, 38, 140, 86, 34, 151, 135, 171, 23, 189,
				2, 148, 135, 22, 128, 20, 11, 18, 242, 44,
				1, 7, 15, 94, 108, 163, 57, 77, 198, 76,
				185, 32
			},
			new byte[32]
			{
				226, 129, 35, 158, 19, 196, 239, 225, 137, 112,
				159, 192, 57, 163, 149, 142, 51, 119, 104, 86,
				36, 230, 102, 86, 5, 90, 217, 219, 50, 187,
				118, 223
			},
			new byte[32]
			{
				161, 20, 161, 252, 174, 27, 193, 18, 124, 8,
				63, 105, 200, 134, 69, 96, 0, 89, 157, 59,
				115, 233, 172, 149, 185, 74, 70, 184, 96, 11,
				143, 165
			},
			new byte[64]
			{
				179, 119, 118, 91, 21, 254, 16, 217, 69, 224,
				160, 31, 165, 89, 242, 196, 134, 27, 243, 147,
				90, 113, 129, 118, 106, 83, 175, 35, 215, 206,
				181, 121, 245, 231, 37, 234, 71, 19, 193, 121,
				47, 17, 168, 80, 167, 77, 151, 5, 186, 240,
				124, 178, 18, 64, 186, 156, 72, 229, 29, 227,
				101, 6, 82, 184
			},
			new byte[48]
			{
				187, 31, 159, 249, 223, 112, 95, 48, 34, 75,
				219, 64, 201, 189, 220, 2, 21, 27, 66, 237,
				74, 3, 64, 89, 195, 252, 54, 200, 24, 118,
				212, 14, 240, 100, 164, 93, 219, 239, 119, 168,
				40, 78, 251, 16, 167, 20, 238, 62
			},
			new byte[48]
			{
				202, 107, 254, 105, 140, 45, 89, 213, 100, 154,
				86, 38, 198, 206, 122, 87, 249, 237, 46, 149,
				73, 214, 224, 64, 1, 134, 216, 115, 218, 66,
				195, 17, 114, 14, 251, 31, 137, 108, 72, 239,
				44, 249, 91, 56, 148, 133, 109, 65
			},
			new byte[48]
			{
				99, 85, 115, 174, 238, 63, 97, 13, 199, 160,
				195, 222, 5, 4, 8, 85, 86, 48, 105, 8,
				31, 253, 179, 188, 242, 139, 138, 36, 239, 104,
				153, 92, 131, 146, 240, 97, 6, 214, 15, 229,
				85, 65, 24, 205, 162, 230, 197, 143
			},
			new byte[32]
			{
				175, 50, 51, 27, 166, 139, 100, 204, 171, 197,
				14, 157, 110, 177, 7, 195, 14, 22, 71, 155,
				52, 114, 129, 248, 146, 190, 48, 230, 216, 204,
				204, 50
			}
		};

		public static bool Filter(DNAGame game, DateTime intendedReleaseDate)
		{
			RBlock rBlock = new RBlock(game, intendedReleaseDate);
			return rBlock.DoFilter();
		}

		public RBlock(DNAGame game, DateTime releaseDate)
		{
			_releaseDate = releaseDate;
			_game = game;
		}

		private void ShowDialog()
		{
			string twoLetterISOLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			int num = 0;
			for (num = 0; num < Cultures.Length && !(Cultures[num] == twoLetterISOLanguageName); num++)
			{
			}
			if (num < Cultures.Length)
			{
				string text = SecurityTools.DecryptString(KeyData, Text1[num]) + "\n\n" + SecurityTools.DecryptString(KeyData, Text2[num]);
				string text2 = SecurityTools.DecryptString(KeyData, Text3[num]);
				_game.Stop = true;
				Guide.BeginShowMessageBox("XNA Game Studio Connect", text, (IEnumerable<string>)new string[1] { text2 }, 0, MessageBoxIcon.Error, (AsyncCallback)End, (object)null);
			}
		}

		public bool DoFilter()
		{
			if (DateTime.Now > _releaseDate)
			{
				return false;
			}
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			for (int i = 0; i < checkStr.Length; i++)
			{
				byte[] code = checkStr[i];
				string key = SecurityTools.DecryptString(KeyData, code);
				dictionary[key] = true;
			}
			if (!Guide.IsVisible)
			{
				for (int j = 0; j < 4; j++)
				{
					try
					{
						SignedInGamer signedInGamer = ((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers)[j];
						if (signedInGamer != null && signedInGamer.IsSignedInToLive && dictionary.ContainsKey(signedInGamer.Gamertag))
						{
							ShowDialog();
							return true;
						}
					}
					catch
					{
					}
				}
			}
			return false;
		}

		private void End(IAsyncResult result)
		{
			_game.Exit();
		}
	}
}
