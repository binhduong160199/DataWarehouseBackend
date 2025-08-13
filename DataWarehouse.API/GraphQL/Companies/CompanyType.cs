using DataWarehouse.Models.Entities;

namespace DataWarehouse.API.GraphQL.Companies;

public class CompanyType : ObjectType<Company>
{
    protected override void Configure(IObjectTypeDescriptor<Company> d)
    {
        d.Name("Company");
        d.Field(x => x.Id).Type<NonNullType<UuidType>>();
        d.Field(x => x.Name).Type<NonNullType<StringType>>();
        d.Field(x => x.Address).Type<StringType>();
        d.Field(x => x.Website).Type<StringType>();
        d.Ignore(x => x.Users);
    }
}