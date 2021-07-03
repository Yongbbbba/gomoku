using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MultiPlayForm : Form
    {
        // 15 x 15 바둑판
        private const int rectSize = 33;  // 오목판의 셀 크기
        private const int edgeCount = 15; // 오목판의 선 개수

        private enum Horse
        {
            none = 0,
            BLACK,
            WHITE
        };
        private Horse[,] board = new Horse[edgeCount, edgeCount];  // 바둑판 2차원 배열로 만들기
        private Horse nowPlayer = Horse.BLACK;  // 흑돌 선공
        private bool playing = false; // 현재 게임이 진행중인 상태인지 체크

        private bool judge() // 승리 판정 함수
        {
            for (int i = 0; i < edgeCount - 4; i++) // 가로
            {
                for (int j = 0; j < edgeCount; j++)
                {
                    if (board[i, j] == nowPlayer && board[i + 1, j] == nowPlayer && board[i + 2, j] == nowPlayer && board[i + 3, j] == nowPlayer && board[i + 4, j] == nowPlayer)
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < edgeCount; i++) // 세로
            {
                for (int j = 4; j < edgeCount; j++)
                {
                    if (board[i, j] == nowPlayer && board[i, j - 1] == nowPlayer && board[i, j - 2] == nowPlayer && board[i, j - 3] == nowPlayer && board[i, j - 4] == nowPlayer)
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < edgeCount - 4; i++)  // Y = X 직선
            {
                for (int j = 0; j < edgeCount - 4; j++)
                {
                    if (board[i, j] == nowPlayer && board[i + 1, j + 1] == nowPlayer && board[i + 2, j + 2] == nowPlayer && board[i + 3, j + 3] == nowPlayer && board[i + 4, j + 4] == nowPlayer)
                    {
                        return true;
                    }
                }
            }

            for (int i = 4; i < edgeCount; i++)  // Y = -X 직선
            {
                for (int j = 0; j < edgeCount - 4; j++)
                {
                    if (board[i, j] == nowPlayer && board[i - 1, j + 1] == nowPlayer && board[i - 2, j + 2] == nowPlayer && board[i - 3, j + 3] == nowPlayer && board[i - 4, j + 4] == nowPlayer)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // 게임 승패가 정해지면 refresh 실행
        private void refresh()
        {
            this.boardPicture.Refresh();
            for (int i = 0; i < edgeCount; i++)
            {
                for (int j = 0; j < edgeCount; j++)
                    board[i, j] = Horse.none;
            }
        }


        public MultiPlayForm()
        {
            InitializeComponent();
            this.playButton.Enabled = false; // 싱글게임과 다르게 혼자 입장했을 때는 게임 시작 버튼을 보이지 않게 함
        }

        private void boardPicture_MouseDown(object sender, MouseEventArgs e)
        {
            if (!playing)
            {
                MessageBox.Show("게임을 실행해주세요.");
                return;
            }
            Graphics g = this.boardPicture.CreateGraphics();  // Graphics 객체 생성
            // 마우스 이벤트가 발생했을 때의 위치
            // 몇 번째 셀인지 확인 (0~14)
            int x = e.X / rectSize;
            int y = e.Y / rectSize;
            if (x < 0 || y < 0 || x >= edgeCount || y >= edgeCount)
            {
                MessageBox.Show("테두리를 벗어날 수 없습니다.");
                return;
            }
            // 이미 돌이 놓인 자리라면 return 시켜서 못 놓게 한다.
            if (board[x, y] != Horse.none)
                return;
            board[x, y] = nowPlayer;
            //MessageBox.Show(x + ", " + y);
            if (nowPlayer == Horse.BLACK)
            {
                // SolidBrush를 이용해서 좌표, 높이, 너비에 맞는 위치를 칠하기
                SolidBrush brush = new SolidBrush(Color.Black);
                g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.White);
                g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
            }

            // 승리 여부 확인
            if (judge())
            {
                status.Text = nowPlayer.ToString() + "플레이어가 승리했습니다.";
                playing = false;
                playButton.Text = "게임시작";
            }
            else
            {
                nowPlayer = ((nowPlayer == Horse.BLACK) ? Horse.WHITE : Horse.BLACK);
                status.Text = nowPlayer.ToString() + " 플레이어의 차례입니다.";
            }
        }

        private void boardPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;
            Color lineColor = Color.Black;  // 오목판 선의 색깔
            Pen p = new Pen(lineColor, 2);
            // 오목판 테두리 그리기
            gp.DrawLine(p, rectSize / 2, rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2); // 좌측
            gp.DrawLine(p, rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize / 2); // 상측
            gp.DrawLine(p, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2);  // 하측
            gp.DrawLine(p, rectSize * edgeCount - rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2);  // 우측
            p = new Pen(lineColor, 1);
            // 대각선 방향으로 이동하면서 십자가 모양의 선 그리기
            for (int i = rectSize + rectSize / 2; i < rectSize * edgeCount - rectSize / 2; i += rectSize)
            {
                gp.DrawLine(p, rectSize / 2, i, rectSize * edgeCount - rectSize / 2, i);
                gp.DrawLine(p, i, rectSize / 2, i, rectSize * edgeCount - rectSize / 2);
            }
        }

        private void enterButton_Click(object sender, EventArgs e)
        {
            this.enterButton.Enabled = false;
            this.playButton.Enabled = true;
            this.status.Text = "[" + this.roomTextBox.Text + "]번 방에 접속했습니다.";
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (!playing)
            {
                refresh();
                playing = true;
                playButton.Text = "재시작";
                status.Text = nowPlayer.ToString() + " 플레이어의 차례입니다.";
            }
            else
            {
                refresh();
                status.Text = "게임이 재시작되었습니다.";
            }
        }
    }
}
