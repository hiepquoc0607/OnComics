using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace OnComics.Infrastructure.Persistence
{
    public class GuidTypeMappingPlugin : IRelationalTypeMappingSourcePlugin
    {
        public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var storeType = mappingInfo.StoreTypeName?.ToLowerInvariant();

            if (storeType == "binary(16)" || storeType == "varbinary(16)")
            {
                return new GuidTypeMapping("binary(16)");
            }

            return null;
        }
    }

    public class DesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            services.AddSingleton<IRelationalTypeMappingSourcePlugin, GuidTypeMappingPlugin>();
        }
    }
}
