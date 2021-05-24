namespace IsUtil.Cache
{
    /// <summary>
    /// 简单的缓存接口，只有查询和添加，以后会进行扩展
    /// </summary>
    public interface ICaching
    {
        T Get<T>(string cacheKey);

        object Get(string cacheKey);
        void Set<T>(string cacheKey, object cacheValue, double timeSpan);

        void Set(string cacheKey, object cacheValue, double timeSpan);

        void Remove(string cacheKey);
    }
}
