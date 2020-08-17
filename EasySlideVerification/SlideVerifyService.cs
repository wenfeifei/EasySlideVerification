﻿using EasySlideVerification.Common;
using EasySlideVerification.Model;
using EasySlideVerification.Store;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;

namespace EasySlideVerification
{
    /// <summary>
    /// 
    /// </summary>
    public class SlideVerifyService : ISlideVerifyService
    {
        IHttpClientFactory httpClientFactory;
        ISlideVerificationStore store;

        public SlideVerifyService(
            IHttpClientFactory httpClientFactory,
            ISlideVerificationStore store)
        {
            this.httpClientFactory = httpClientFactory;
            this.store = store;
        }

        /// <summary>
        /// 创建图片滑动数据
        /// </summary>
        public SlideVerificationInfo Create()
        {
            SlideVerificationParam param = new SlideVerificationParam();
            param.BackgroundImage = BackgroundImageProvider.Instance.GetRandomOne();
            param.SlideImage = SlideImageProvider.Instance.GetRandomOne();
            param.Edge = SlideVerificationOptions.Default.Edge;
            param.MixedCount = SlideVerificationOptions.Default.MixedCount;
            SlideVerificationInfo result = SlideVerificationCreater.Instance.Create(param);

            this.store.Add(result, SlideVerificationOptions.Default.Expire);

            return result;
        }

        /// <summary>
        /// 验证结果
        /// </summary>
        /// <param name="key"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Validate(string key, int x, int y)
        {
            var data = this.store.Get(key);
            if (data == null) return false;

            int accept = SlideVerificationOptions.Default.AcceptableDeviation;

            return x > data.OffsetX - accept && x < data.OffsetX + accept
                && y > data.OffsetY - accept && y < data.OffsetY + accept;
        }

    }
}