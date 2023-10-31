using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiUniversidade.Model;
using apiUniversidade.Context;
using Microsoft.AspNetCore.Mvc;

namespace apiUniversidade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisciplinaController  : ControllerBase
    {
        private readonly ILogger<AlunoController> _logger;
        private readonly ApiUniversidadeContext _context; 

        public DisciplinaController (ILogger<DisciplinaController> logger, ApiUniversidadeContext context)
        {
            _logger = logger;
            _context = context;
        }

    [HttpGet]
        public ActionResult<IEnumerable<Aluno>> Get()
        {
            var disciplinas = _context.Disciplinas.ToList();
            if(disciplinas is null)
                return NotFound();
            
            return disciplinas;
        }

        [HttpGet("{id:int}", Name ="GetDisciplinas")]
        public ActionResult<Disciplina> Get(int id)
        {
            var disciplina = _context.Disciplinas.FirstOrDefault(p => p.Id == id);
            if(disciplina is null)
                return NotFound("Disciplina não encontrada");
            
            return disciplina;
        }

        
        [HttpPost]
        public ActionResult Post(Disciplina disciplinas){
            _context.Disciplinas.Add(disciplinas);
            _context.SaveChanges();

            return new CreatedAtRouteResult("GetDisciplinas",
                new{id = disciplinas.Id},
                disciplinas);
                
        }
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Disciplina disciplina){
            if(id != disciplina.Id)
                return BadRequest();

            _context.Entry(disciplina).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return Ok(disciplina);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id){
            var disciplina = _context.Disciplinas.FirstOrDefault(p => p.Id == id);
            
            if(disciplina is null) 
                return NotFound();

            _context.Disciplinas.Remove(disciplina);
            _context.SaveChanges();

            return Ok(disciplina);
        }

        

    }
}
