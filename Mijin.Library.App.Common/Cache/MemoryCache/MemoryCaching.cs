﻿using Microsoft.Extensions.Caching.Memory;
using System;

namespace IsUtil.Cache
{
    /// <summary>
    /// 实例化缓存接口ICaching
    /// </summary>
    public class MemoryCaching : ICaching
    {
        //引用Microsoft.Extensions.Caching.Memory;这个和.net 还是不一样，没有了Httpruntime了
        private readonly IMemoryCache _cache;
        //还是通过构造函数的方法，获取
        public MemoryCaching(IMemoryCache cache)
        {
            _cache = cache;
        }
        public object Get(string cacheKey)
        {
            return _cache.Get(cacheKey);
        }
        public T Get<T>(string cacheKey)
        {
            return _cache.Get<T>(cacheKey);
        }
        public void Set(string cacheKey, object cacheValue, double timeSpan)
        {
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(timeSpan * 60));
        }

        public void Set<T>(string cacheKey, object cacheValue, double timeSpan)
        {
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(timeSpan * 60));
        }

        public void Remove(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }
    }

}
