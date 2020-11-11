using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IMapper _mapper;
        private readonly INotifier _notifier;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;

        public FornecedoresController(IMapper mapper,
            INotifier notifier,
            IFornecedorRepository fornecedorRepository,
            IFornecedorService fornecedorService)
        {
            _mapper = mapper;
            _notifier = notifier;
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
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
            if (!ModelState.IsValid) return BadRequest();

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorView);
            await _fornecedorService.Adicionar(fornecedor);

            if (!_notifier.IsValid()) return BadRequest(_notifier.Get());

            return fornecedorView;
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorView)
        {
            if (id != fornecedorView.Id || !ModelState.IsValid) return BadRequest();

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorView);
            await _fornecedorService.Atualizar(fornecedor);

            if (!_notifier.IsValid()) return BadRequest(_notifier.Get());

            return fornecedorView;
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Delete(Guid id)
        {
            await _fornecedorService.Remover(id);

            var fornecedor = _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterPorId(id));

            if (!_notifier.IsValid()) return BadRequest(_notifier.Get());

            return Ok(fornecedor);
        }
    }
}
