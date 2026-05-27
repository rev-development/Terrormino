using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class AudioController : MonoBehaviour
    {

        public List<AudioSource> LineClearedSound = new();
        public AudioSource TetrisLoseSound;
        public AudioSource PieceSpawnedSound;
        public AudioSource PieceMovedSound;
        public List<AudioSource> PieceRotatedSounds = new();
        public List<AudioSource> PieceLocked = new();


        public Board Board;
        public ActivePieceController PieceController;

        public void Start()
        {
            Board = Board.Instance;
        }

        public void OnEnable()
        {
            Board.LineClearedEvent.AddListener(OnLineCleared);
            Board.TetrisLose.AddListener(() => OnTetrisEvent(TetrisLoseSound));
            Board.PieceSpawned.AddListener(() => OnTetrisEvent(PieceSpawnedSound));
            PieceController.PieceMoved.AddListener(() => OnTetrisEvent(PieceMovedSound));
            PieceController.PieceRotated.AddListener(OnPieceRotated);
            PieceController.PieceLocked.AddListener(OnPieceLocked);
        }

        public void OnTetrisEvent(AudioSource audioSource)
        {
            if (audioSource)
            {
                audioSource.Play();
            }
        }

        private void OnPieceRotated()
        {
            if (PieceRotatedSounds == null || PieceRotatedSounds.Count == 0) return;
            var sound = PieceRotatedSounds[Random.Range(0, PieceRotatedSounds.Count)];
            if (sound != null) sound.Play();
        }

        private void OnPieceLocked()
        {
            if (PieceLocked == null || PieceLocked.Count == 0) return;
            var sound = PieceLocked[Random.Range(0, PieceLocked.Count)];
            if (sound != null) sound.Play();

        }

        private void OnLineCleared()
        {
            if (LineClearedSound == null || LineClearedSound.Count == 0) return;
            var sound = LineClearedSound[Random.Range(0, LineClearedSound.Count)];
            if (sound != null) sound.Play();
        }



    }
}