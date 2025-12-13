using System;
using System.Linq;
using OctaNotes.Scripts.Play.Interface;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class NoteGenerator
    {
        [Inject] private IChartRepositoryImmutable _chartRepository;
        
        
    }
}
