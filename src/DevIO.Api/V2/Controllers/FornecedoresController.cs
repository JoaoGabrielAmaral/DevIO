using AutoMapper;
using DevIO.Api.Controller;
using DevIO.Api.ViewModels;
using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.V2.Controllers
{
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IMapper _mapper;
        private readonly IFornecedorRepository _fornecedorRepository;

        public FornecedoresController(
            INotifier notifier,
            IUser user,
            IMapper mapper,
            IFornecedorRepository fornecedorRepository
        ) : base(notifier, user)
        {
            _mapper = mapper;
            _fornecedorRepository = fornecedorRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> Find()
        {
            var fornecedor = _mapper.Map<IEnumerable<FornecedorViewModel>>(
                await _fornecedorRepository.ObterTodos()
            );

            return fornecedor;
        }

        [HttpGet("teste")]
        public void Teste()
        {
            var div = 0;
            var result = 120 / div;
        }
    }
}
