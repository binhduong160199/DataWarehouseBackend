using DataWarehouse.Models.Entities;

namespace DataWarehouse.API.GraphQL.Companies
{
    public class CompanyType : ObjectType<Company>
    {
        protected override void Configure(IObjectTypeDescriptor<Company> d)
        {
            d.Name("Company");

            d.Field(x => x.Id).Type<NonNullType<UuidType>>();
            d.Field(x => x.Name).Type<NonNullType<StringType>>();
            d.Field(x => x.Address).Type<StringType>();
            d.Field(x => x.Website).Type<StringType>();
            d.Field(x => x.ContactEmail).Type<StringType>();
            d.Field(x => x.PhoneNumber).Type<StringType>();
            d.Field(x => x.Industry).Type<StringType>();
            d.Field(x => x.TaxId).Type<StringType>();
            d.Field(x => x.LogoUrl).Type<StringType>();
            d.Field(x => x.IsActive).Type<BooleanType>();
            d.Field(x => x.CreatedAt).Type<DateTimeType>();
            d.Field(x => x.UpdatedAt).Type<DateTimeType>();

            d.Ignore(x => x.Users); // still ignore navigation property
        }
    }
}