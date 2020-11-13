using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IMapper _mapper;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IEnderecoRepository _enderecoRepository;

        public FornecedoresController(IMapper mapper,
            INotifier notifier,
            IFornecedorRepository fornecedorRepository,
            IFornecedorService fornecedorService,
            IEnderecoRepository enderecoRepository) : base(notifier)
        {
            _mapper = mapper;
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _enderecoRepository = enderecoRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> Obter()
        {
            var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return fornecedores;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Obter(Guid id)
        {
            var fornecedor = _mapper.Map<FornecedorViewModel>(
                await _fornecedorRepository.ObterFornecedorProdutosEndereco(id)
            );
            if (fornecedor == null) return NotFound();

            return fornecedor;
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorView)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorView);
            await _fornecedorService.Adicionar(fornecedor);

            return CustomResponse(_mapper.Map<FornecedorViewModel>(fornecedor));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorView)
        {
            if (id != fornecedorView.Id)
            {
                NotifyError("O id informado não é o mesmo que foi passado na query");
                return CustomResponse(fornecedorView);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorView));

            return CustomResponse(fornecedorView);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Delete(Guid id)
        {
            var fornecedor = await _fornecedorRepository.ObterPorId(id);
            if (fornecedor == null) return NotFound();

            await _fornecedorService.Remover(id);

            return CustomResponse(_mapper.Map<FornecedorViewModel>(fornecedor));
        }

        [HttpPut("{fornecedorId:guid}/address/{id:guid}")]
        public async Task<ActionResult> UpdateAdress(Guid fornecedorId, Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (fornecedorId != enderecoViewModel.FornecedorId)
            {
                NotifyError("O id do fornecedor informado não é o mesmo que foi passado na query.");
                return CustomResponse(enderecoViewModel);
            }

            if (id != enderecoViewModel.Id)
            {
                NotifyError("O id do endereço informado não é o mesmo que foi passado na query.");
                return CustomResponse(enderecoViewModel);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var endereco = await _enderecoRepository.ObterPorId(id);
            if (endereco == null) return NotFound();

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoViewModel));

            return CustomResponse(enderecoViewModel);
        }
    }
}
