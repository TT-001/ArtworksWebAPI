using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Models;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtTypesController : ControllerBase
    {
        private readonly ArtsContext _context;

        public ArtTypesController(ArtsContext context)
        {
            _context = context;
        }

        // GET: api/ArtTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArtTypeDTO>>> GetArtTypes()
        {
            return await _context.ArtTypes
                .Select(d => new ArtTypeDTO
                {
                    ID = d.ID,
                    Type = d.Type
                    
                })
                .ToListAsync();
        }

        [HttpGet("Inc")]
        public async Task<ActionResult<IEnumerable<ArtTypeDTO>>> GetArtTypesInc()
        {
            return await _context.ArtTypes.Include(a => a.Artworks)
                .Select(d => new ArtTypeDTO
                {
                    ID = d.ID,
                    Type = d.Type,
                    Artworks = d.Artworks.Select(dArtwork => new ArtworkDTO
                    {
                        ID = dArtwork.ID,
                        Name = dArtwork.Name,
                        Completed = dArtwork.Completed,
                        Description = dArtwork.Description,
                        Value = dArtwork.Value,
                        ArtTypeID = dArtwork.ArtTypeID
                        
                    }).ToList()
                })
                .ToListAsync();
        }

        // GET: api/ArtTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArtTypeDTO>> GetArtType(int id)
        {
            var artTypeDTO = await _context.ArtTypes
                .Select(d => new ArtTypeDTO
                {
                    ID = d.ID,
                    Type = d.Type

                })
                .FirstOrDefaultAsync(d => d.ID == id);

            if (artTypeDTO == null)
            {
                return NotFound();
            }

            return artTypeDTO;
        }

        // GET: api/ArtTypes/inc/5
        [HttpGet("inc/{id}")]
        public async Task<ActionResult<ArtTypeDTO>> GetArtTypeInc(int id)
        {
            var artTypeDTO = await _context.ArtTypes.Include(d => d.Artworks)
                .Select(d => new ArtTypeDTO
                {
                    ID = d.ID,
                    Type = d.Type,
                    Artworks = d.Artworks.Select(dArtwork => new ArtworkDTO
                    {
                        ID = dArtwork.ID,
                        Name = dArtwork.Name,
                        Completed = dArtwork.Completed,
                        Description = dArtwork.Description,
                        Value = dArtwork.Value,
                        ArtTypeID = dArtwork.ArtTypeID

                    }).ToList()

                })
                .FirstOrDefaultAsync(d => d.ID == id);

            if (artTypeDTO == null)
            {
                return NotFound();
            }

            return artTypeDTO;
        }

        // PUT: api/ArtTypes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArtType(int id, ArtTypeDTO artTypeDTO)
        {
            if (id != artTypeDTO.ID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Get the record you want to update
            var artTypeToUpdate = await _context.ArtTypes.FindAsync(id);

            //Check that you got it
            if (artTypeToUpdate == null)
            {
                return BadRequest(new { message = "Error: Record not found." });
            }

            artTypeToUpdate.ID = artTypeDTO.ID;
            artTypeToUpdate.Type = artTypeDTO.Type;
            

            //Wow, we have a chance to check for concurrency even before bothering
            //the database!  Of course, it will get checked again in the database just in case
            //it changes after we pulled the record.  
            //Note using SequenceEqual becuase it is an array after all.
            //if (!artTypeToUpdate.RowVersion.SequenceEqual(artTypeDTO.RowVersion))
            //{
            //    return BadRequest(new { message = "Concurrency Error: Artwork has been changed by another user.  Back out and try editing the record again." });
            //}

            


            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(artTypeToUpdate).Property("RowVersion").OriginalValue = artTypeDTO.RowVersion;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtTypeExists(id))
                {
                    return BadRequest(new { message = "Concurrency Error: Art Type has been Removed." });
                }
                else
                {
                    return BadRequest(new { message = "Concurrency Error: Art Type has been updated by another user.  Back out and try editing the record again." });
                }
            }
        }

        // POST: api/ArtTypes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ArtType>> PostArtType(ArtTypeDTO artTypeDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ArtType artType = new ArtType
            {
                
                Type = artTypeDTO.Type
                
            };

            try
            {
                _context.ArtTypes.Add(artType);
                await _context.SaveChangesAsync();
                //Assign Database Generated values back into the DTO
                artTypeDTO.ID = artType.ID;
                return CreatedAtAction(nameof(GetArtType), new { id = artType.ID }, artTypeDTO);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
            }
        }

        // DELETE: api/ArtTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ArtType>> DeleteArtType(int id)
        {
            var artType = await _context.ArtTypes.FindAsync(id);
            if (artType == null)
            {
                return BadRequest(new { message = "Delete Error: Art Type has already been removed." });
            }
            try
            {
                _context.ArtTypes.Remove(artType);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    return BadRequest(new { message = "Delete Error: Remember, you cannot delete ArtType thas artworks assigned." });
                }
                else
                {
                    return BadRequest(new { message = "Delete Error: Unable to delete ArtType. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        private bool ArtTypeExists(int id)
        {
            return _context.ArtTypes.Any(e => e.ID == id);
        }
    }
}
