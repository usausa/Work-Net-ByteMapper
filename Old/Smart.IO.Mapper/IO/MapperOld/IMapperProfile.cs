﻿namespace Smart.IO.MapperOld
{
    /// <summary>
    ///
    /// </summary>
    public interface IMapperProfile
    {
        /// <summary>
        ///
        /// </summary>
        string Name { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        void Configure(IMapperConfigurationExpresion config);
    }
}