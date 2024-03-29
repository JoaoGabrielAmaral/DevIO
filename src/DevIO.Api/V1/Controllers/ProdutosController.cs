﻿using AutoMapper;
using DevIO.Api.Controller;
using DevIO.Api.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevIO.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutosController : MainController
    {
        private readonly IMapper _mapper;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;

        public ProdutosController(IMapper mapper,
            INotifier notifier,
            IUser user,
            IProdutoRepository produtoRepository,
            IProdutoService produtoService) : base(notifier, user)
        {
            _mapper = mapper;
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoViewModel>>> Get()
        {
            return CustomResponse(
                _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores())
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Get(Guid id)
        {
            var produto = await _produtoRepository.ObterProdutoFornecedor(id);
            if (produto == null) return NotFound();

            return CustomResponse(
                _mapper.Map<ProdutoViewModel>(produto)
            );
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Create(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var fileName = $"{Guid.NewGuid()}_{produtoViewModel.Imagem}";
            if (!UploadFile(fileName, produtoViewModel.ImagemUpload))
            {
                return CustomResponse(produtoViewModel);
            }

            produtoViewModel.Imagem = fileName;
            var produto = _mapper.Map<Produto>(produtoViewModel);
            await _produtoService.Adicionar(produto);

            return CustomResponse(_mapper.Map<ProdutoViewModel>(produto));
        }

        //Configuracao para base64 mt grandes
        [RequestSizeLimit(400000000)]
        [HttpPost("upload-image")]
        public async Task<ActionResult> Create(ImageProdutoViewModel imageProdutoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var prefix = $"{Guid.NewGuid()}_";
            if (!await UploadFile(prefix, imageProdutoViewModel.ImagemUpload))
            {
                return CustomResponse(imageProdutoViewModel);
            }

            imageProdutoViewModel.Imagem = $"{prefix}{imageProdutoViewModel.ImagemUpload.FileName}";
            var produto = _mapper.Map<Produto>(imageProdutoViewModel);
            await _produtoService.Adicionar(produto);

            return CustomResponse(_mapper.Map<ProdutoViewModel>(produto));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Update(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)
            {
                NotifyError("O id informado não é o mesmo que foi passado na query.");
                return CustomResponse();
            }

            var produtoAtualizacao = await _produtoRepository.ObterPorId(id);
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (produtoViewModel.ImagemUpload != null)
            {
                var fileName = $"{Guid.NewGuid()}_{produtoViewModel.Imagem}";
                if (!UploadFile(fileName, produtoViewModel.ImagemUpload))
                {
                    return CustomResponse(produtoViewModel);
                }

                produtoAtualizacao.Imagem = fileName;
            }

            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            return CustomResponse(produtoViewModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Remover(Guid id)
        {
            var produto = await _produtoRepository.ObterPorId(id);
            if (produto == null) return NotFound();

            await _produtoService.Remover(id);

            return CustomResponse();
        }

        private bool UploadFile(string fileName, string file)
        {
            var fileBase = Convert.FromBase64String(file);
            if (string.IsNullOrEmpty(file))
            {
                NotifyError("Forneça uma imagem para este produto!");
                return false;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            if (System.IO.File.Exists(filePath))
            {
                NotifyError("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, fileBase);

            return true;
        }

        private async Task<bool> UploadFile(string prefix, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                NotifyError("Forneça uma imagem para este produto!");
                return false;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{prefix}{file.FileName}");
            if (System.IO.File.Exists(path))
            {
                NotifyError("Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return true;
        }
    }
}
