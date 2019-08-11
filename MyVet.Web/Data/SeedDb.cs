using MyVet.Web.Data.Entities;
using MyVet.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        //private readonly IAgendaHelper _agendaHelper;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
            //_agendaHelper = agendaHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRoles();
            var manager = await CheckUserAsync("1010", "Juan", "Zuluaga", "jzuluaga55@gmail.com", "350 634 2747", "Calle Luna Calle Sol", "Admin");
            var customer = await CheckUserAsync("2020", "Juan", "Zuluaga", "jzuluaga55@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", "Customer");
            await CheckPetTypesAsync();
            await CheckServiceTypesAsync();
            await CheckOwnerAsync(customer);
            await CheckManagerAsync(manager);
            await CheckPetsAsync();
            //await CheckAgendasAsync();
        }

        private async Task CheckRoles()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");
        }

        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, string role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, role);
            }

            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            await _userHelper.ConfirmEmailAsync(user, token);

            return user;
        }

        private async Task CheckPetsAsync()
        {
            if (!_dataContext.Pets.Any())
            {
                var owner = _dataContext.Owners.FirstOrDefault();
                var petType = _dataContext.PetTypes.FirstOrDefault();
                AddPet("Otto", owner, petType, "Shih tzu");
                AddPet("Killer", owner, petType, "Dobermann");
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckServiceTypesAsync()
        {
            if (!_dataContext.ServiceTypes.Any())
            {
                _dataContext.ServiceTypes.Add(new ServiceType { Name = "Consulta" });
                _dataContext.ServiceTypes.Add(new ServiceType { Name = "Urgencia" });
                _dataContext.ServiceTypes.Add(new ServiceType { Name = "Vacunación" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckPetTypesAsync()
        {
            if (!_dataContext.PetTypes.Any())
            {
                _dataContext.PetTypes.Add(new PetType { Name = "Perro" });
                _dataContext.PetTypes.Add(new PetType { Name = "Gato" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckOwnerAsync(User user)
        {
            if (!_dataContext.Owners.Any())
            {
                _dataContext.Owners.Add(new Owner { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckManagerAsync(User user)
        {
            if (!_dataContext.Managers.Any())
            {
                _dataContext.Managers.Add(new Manager { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private void AddPet(string name, Owner owner, PetType petType, string race)
        {
            var histories = new List<History>
            {
                new History
                {
                    Date = DateTime.Now,
                    Description = "Consulta",
                    Remarks = "Fusce gravida convallis tortor, non lobortis massa. Duis hendrerit mauris et lectus dapibus finibus. Etiam dictum molestie tortor et tincidunt. Nam viverra nunc vitae leo porta, et dapibus dui ultrices.",
                    ServiceType = _dataContext.ServiceTypes.FirstOrDefault()
                },
                new History
                {
                    Date = DateTime.Now,
                    Description = "Consulta",
                    Remarks = "Maecenas quis molestie sem, at convallis magna. Vestibulum euismod augue eu erat fringilla tempus. Phasellus vel ante interdum, bibendum tortor quis, sodales ex.",
                    ServiceType = _dataContext.ServiceTypes.FirstOrDefault()
                },
                new History
                {
                    Date = DateTime.Now,
                    Description = "Consulta",
                    Remarks = "Quisque dapibus semper diam, vitae bibendum ex volutpat et. Proin eu posuere augue. Nulla at nisi purus. Proin a scelerisque orci. Ut sapien erat, tempor ac ligula sit amet, lobortis laoreet arcu.",
                    ServiceType = _dataContext.ServiceTypes.FirstOrDefault()
                }
            };

            _dataContext.Pets.Add(new Pet
            {
                Born = DateTime.Now.AddYears(-2),
                Name = name,
                Owner = owner,
                PetType = petType,
                Race = race,
                ImageUrl = $"~/images/Pets/{name}.png",
                Remarks = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas non tempus velit. Vestibulum nec vehicula urna, quis tincidunt diam. In vitae ultricies ipsum.",
                Histories = histories
            });
        }

        //private async Task CheckAgendasAsync()
        //{
        //    if (!_dataContext.Agendas.Any())
        //    {
        //        //await _agendaHelper.AddDays(30);
        //    }
        //}
    }
}
