using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;


// Endpoint => URL
// https://localhost:5001/banana/categories

[Route("v1/categories")]
public class CategoryController: ControllerBase{

    /*
     * Como navegamos nesse controller/classe
     * e como acessamos metodos do mesmo
     * Para tal recorremos ao conceito de rotas

    */

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
    {
       // return new List<Category>();
       return  await context.Categories.AsNoTracking().ToListAsync(); 
    }


    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(int id,
     [FromServices] DataContext context)
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c=>c.Id ==id);
        return category;
    }



    [HttpPost]
    [Route("")]
    [Authorize("manager")]
    public async Task<ActionResult<Category>> Post(
        [FromBody]Category model,
        [FromServices] DataContext context)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {

            context.Categories.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);

        }
        catch 
        {

            return BadRequest(new {message="Não foi possível criar a categoria"});
            
        }

    }


    [HttpPut]
    [Route("{id:int}")]
    [Authorize("manager")]
    public async Task<ActionResult<Category>> Put(int id, 
        [FromBody] Category model,
        [FromServices] DataContext context)
    {

        if (model.Id != id)
            return NotFound(new {message = "Categoria não encontrada" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);

        }
        catch(DbUpdateConcurrencyException)
        {

            return BadRequest(new { message = "Este registo já foi actualizado" });

        }

        catch (Exception)
        {

            return BadRequest(new { message = "Não foi possível criar a categoria" });

        }


    }


    [HttpDelete]
    [Route("{id:int}")]
    [Authorize("manager")]
    public async Task<ActionResult<Category>> Delete(int id,
        [FromServices]DataContext context)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return NotFound(new {message = "Categoria não encontrada" });

        try
        {
            context.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new { message="Categoria removida com sucesso!"});

        }
        catch (Exception)
        {
            return BadRequest(new { message= "Ocorreu um erro ao remover a categoria."});
        }


    }




}


/* Iniciando Controller de Produtos
 * 
 * 
 * 
 * 
 * /

/* Get e GetById
 * Metodos de leitura
 * proxy da categoria EF cria isso com informações desse item
 * Uso do AsNoTracking para evitar carregar detalhes EF que cria ao realizarmos consultas
 * Tudo que for leitura e não precisarmos de usar no metodo em questão usamos o metodo acima para acelarar a rapidez da consulta. 
 * 
 * /

/* Actualizando uma categoria e Removendo uma categoria
 * Seguimos a mesma logica do POST, só que aqui acrescetamos
 * o metodo Entry(model).State 
 * 
 * Fazemos a busca do id
 * Removemos essa categoria
 * 
 * /

/* Manipulando Erros
 * Recorremos ao try catch para tratar erros de acesso ao nosso DataSet
 * É algo importante para questões de base de dados( a base pode estar em baixo, podemos ter algum problema durante a inserção de um registo)...
 *
 *
 *
 */


/* Dependecy Injection
 *
 * Informar a nossa app sobre a existencia do DBcontext informamos a app a existencia desse DataContext
 * Tornar o DBContext disponivel para os nosso controllers
 * Para os  nossos controllers funcionarem eles precisam do DBContext
 * E como isso é uma dependencia precisamos resolver ela
 *
 * Os nosso controllers dependem do DBContext para isso devemos
 * 1. registar esse serviço no metodo ConfigureServices atarves do metodo AddDbContext<DataContext>(Inf. das base de dados) 
 * 2. precisamos tornar disponivel esse datacontext para os nossos controllers recorrendo ao (DI)
 * 3. mas para os nossos controllers funcionarem eles necessitam ou dependem dos nosso datacontext e nos metodos iremos passar eles
 * mas as nossas classes não precisam se preocupar como irão receber esse datacontext eles apenas precisam disso
 * 4. Como resolver essa dependencia sem criar multiplas conexões no banco de dados?
 * No modelo de API, cada requisição que fazemos ela executada e depois o usuario  é desligado dessa API.
 * Ou seja essa API deve retornar ao usuario e fechar essa conexão, não podemos deixar nada aberto.
 * Não podemos deixar nada aberto pois o banco tem um limite de conexões.
 * 5. Na gestão das conexões, muitas das vezes podemos esquecer de fecha-lá, se é algo importante deixar na nossa app
 * pre-configurado.
 *
 */


/* DataContext
 * dotnet add package Microsoft.EntityFrameworkCore.InMemory
 *dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 3.1.0 
 * Iremos trabalhar em Memoria para entender o conceito de DataContext
 *
 *
 *
 *
 */




/* Task e ActionResults
 * Programação paralea basta colocar os metodos com palavra Task
 * ActionResult faz parte ControllerBase e ele já trás o resultado que a tela espera
 * aysnc assinatura do metodo para poder ser um metodo anssincrono 
 */


/* Capturando o JSON enviado
 * Para o caso do GET, os parametros via url nos mapeamos, através da Rota.
 *  
 * Mas no caso do POST, para montarmos uma requisição, essa requisição ela é composta de 2 itens: cabeçalho e corpo
 * Capturar o JSON enviado para nossa app
 * Para tal é necessario definirmos um formato para ele, não podemos apenas postar qualquer coisa para categoria e esperar que as coisas funcionem
 * Para tal é necessario postar informações correctas "Models Category"
 * Tem um promenor, quando criamos parametros na rota, o .net sabe que estamos na rota
 * Mas para o caso do POST que recorremos ao nosso modelo, ele não sabe de onde buscar essa informação "Parametro Category no nosso metodo".
 * Sob o nosso parametro adicionamos "[FromBody]", assim informamos ao nosso parametro que vem uma categoria do corpo da nossa requisição.
 * Model Binder que ira ligar o json com o model
 *
 */


/* Parametros via URL
 * Tudo que tivermos entre {} na rota, do net.core irá encarar como parametro.
 * Para passar o parametro da rota para o nosso metodo, devemos apenas escrever o nome desse
 * parametro como vem nas {}
 * Além disso podemos colocar validações basicas na rotas(restrição de rota), por exemplo uma rota que espera por um
 * pela passagem de um inteiro e colocamos uma string em certos casos se não tratarmos isso, podem ser geradas expeções após a
 * execução do método, para tal é necessario sob o atributo ROUTE("{id:int}") fazer esse acrescimo.
 * Usamos isso para evitar o tratamento de erros provinietes dessa requisição. 
 
 */



/*
 * Rotas - Rest 
 * CRUD da app
 * Para podermos fazer essas operações devemos sempre ter a mesma rota
 * Para o caso das categorias, teremos uma mesma rota para podermos realizar
 * as operações.
 * Por padrão, para esses cenários o REST tem sempre a mesma rota.
 * Ou seja  essa rota "// https://localhost:5001/banana/categories"
 * irá servir para o CREATE,UPDATE,DELETE e READ
 * Mas como o ASP.NET identifica cada uma dessas operações?
 * Toda vez que chamamos ou fazemos essa requisição "// https://localhost:5001/banana/categories", no postman tem um verbo associado a mesma.
 * E temos uma gama de verbos : GET,POST,PUT e DELETE esses são os mais conhecidos
 * Convenções: 
 * Se não especificamos nada no metodo por padrão ele é HTTPGET
 * Podemos decorrar ou não  o metodo os verbos
 * É sempre a mesma rota, oque troca apenas é o verbo...
 */