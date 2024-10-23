using System;
using AutoMapper;
using ServicioClientes.Aplicacion.DTOs;
using ServicioClientes.Dominio.Entidades;

namespace ServicioClientes.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cliente, ClienteDto>().ReverseMap();
            CreateMap<Persona, PersonaDto>().ReverseMap();
        }
    }
}

