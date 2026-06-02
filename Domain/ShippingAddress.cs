using System;

namespace Amplifike.Domain
{
    public class ShippingAddress : BaseEntity
    {
        public Guid PledgeId { get; private set; }
        public string PostalCode { get; private set; }
        public string Street { get; private set; }
        public string Number { get; private set; }
        public string? Complement { get; private set; }
        public string District { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }

        public ShippingAddress(Guid pledgeId, string postalCode, string street, string number, string district, string city, string state, string? complement = null, string country = "Brasil")
        {
            if (pledgeId == Guid.Empty)
                throw new ArgumentException("Id do apoio inválido.", nameof(pledgeId));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("CEP é obrigatório.", nameof(postalCode));

            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Rua é obrigatória.", nameof(street));

            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Número é obrigatório.", nameof(number));

            if (string.IsNullOrWhiteSpace(district))
                throw new ArgumentException("Bairro é obrigatório.", nameof(district));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("Cidade é obrigatória.", nameof(city));

            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("Estado é obrigatório.", nameof(state));

            PledgeId = pledgeId;
            PostalCode = postalCode;
            Street = street;
            Number = number;
            Complement = complement;
            District = district;
            City = city;
            State = state;
            Country = country;
        }
    }
}
