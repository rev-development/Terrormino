using UnityEngine;

namespace Tetris
{
    public class AudioController : MonoBehaviour
    {

        public AudioSource LineClearedSound;
        public AudioSource TetrisLoseSound;
        public AudioSource PieceSpawnedSound;
        public AudioSource PieceMovedSound;
        public AudioSource PieceRotatedSound;
        public AudioSource PieceLockedSound;

        public Board Board;

        public void Start()
        {
            Board = Board.Instance;
        }

        public void OnEnable()
        {
            Board.LineCleared.AddListener(_ => OnTetrisEvent(LineClearedSound));
            Board.TetrisLose.AddListener(() => OnTetrisEvent(TetrisLoseSound));
            Board.PieceSpawned.AddListener(() => OnTetrisEvent(PieceSpawnedSound));
            Board.ActivePiece.PieceMoved.AddListener(() => OnTetrisEvent(PieceMovedSound));
            Board.ActivePiece.PieceRotated.AddListener(() => OnTetrisEvent(PieceRotatedSound));
            Board.ActivePiece.PieceLocked.AddListener(() => OnTetrisEvent(PieceLockedSound));
        }

        public void OnTetrisEvent(AudioSource audioSource)
        {
            if (audioSource)
            {
                audioSource.Play();
            }
        }

    }
}