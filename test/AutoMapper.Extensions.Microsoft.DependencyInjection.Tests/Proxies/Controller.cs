namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;

    internal class Controller
    {
        private readonly IConverter _converter;
        private readonly IConverter1 _converter1;
        private readonly IConverter2 _converter2;
        private readonly IConverter3 _converter3;
        private readonly IConverter4 _converter4;
        private readonly IConverter5 _converter5;

        public Controller(IConverter converter, IConverter1 converter1, IConverter2 converter2, IConverter3 converter3, IConverter4 converter4, IConverter5 converter5)
        {
            _converter = converter;
            _converter1 = converter1;
            _converter2 = converter2;
            _converter3 = converter3;
            _converter4 = converter4;
            _converter5 = converter5;
        }

        public string DoIt()
        {
            var good = new Person { Name = "Tom" };

            var bad1 = _converter.Map1<Pirate>(good);
            if (bad1 == null || bad1.Name != "Pirate " + good.Name)
                throw new ApplicationException();

            var bad11 = _converter1.Map1<Pirate>(good);
            if (bad11 == null || bad11.Name != "Pirate " + good.Name)
                throw new ApplicationException();



            var bad2 = _converter.Map2<Person, Pirate>(good);
            if (bad2 == null || bad2.Name != "Pirate " + good.Name)
                throw new ApplicationException();

            var bad22 = _converter2.Map2<Person, Pirate>(good);
            if (bad22 == null || bad22.Name != "Pirate " + good.Name)
                throw new ApplicationException();



            var bad3 = _converter.Map3(good, new Pirate());
            if (bad3 == null || bad3.Name != "Pirate " + good.Name)
                throw new ApplicationException();

            var bad33 = _converter3.Map3(good, new Pirate());
            if (bad33 == null || bad33.Name != "Pirate " + good.Name)
                throw new ApplicationException();



            var bad4 = (Pirate)_converter.Map4(good, typeof(Person), typeof(Pirate));
            if (bad4 == null || bad4.Name != "Pirate " + good.Name)
                throw new ApplicationException();

            var bad44 = (Pirate)_converter4.Map4(good, typeof(Person), typeof(Pirate));
            if (bad44 == null || bad44.Name != "Pirate " + good.Name)
                throw new ApplicationException();



            var bad5 = (Pirate)_converter.Map5(good, new Pirate(), typeof(Person), typeof(Pirate));
            if (bad5 == null || bad5.Name != "Pirate " + good.Name)
                throw new ApplicationException();

            var bad55 = (Pirate)_converter5.Map5(good, new Pirate(), typeof(Person), typeof(Pirate));
            if (bad55 == null || bad55.Name != "Pirate " + good.Name)
                throw new ApplicationException();



            return "OK!";
        }
    }
}
