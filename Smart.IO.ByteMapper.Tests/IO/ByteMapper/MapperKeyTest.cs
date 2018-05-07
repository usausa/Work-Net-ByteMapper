﻿namespace Smart.IO.ByteMapper
{
    using Xunit;

    public class MapperKeyTest
    {
        [Fact]
        public void MapKeyCompare()
        {
            var key1 = new MapperKey(typeof(object), string.Empty);
            var key1B = key1;

            // Compare to null
            Assert.False(key1.Equals(null));

            // Compar to self
            Assert.True(key1.Equals(key1B));

            // Compar to same
            Assert.True(key1.Equals(new MapperKey(typeof(object), string.Empty)));

            // Compar to different type
            Assert.False(key1.Equals(new MapperKey(typeof(string), string.Empty)));

            // Compar to different name
            Assert.False(key1.Equals(new MapperKey(typeof(object), "x")));

            // Compar to another type
            Assert.False(key1.Equals(new object()));
        }
    }
}