using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    #region Unity Callbacks

    private void Awake()
    {
        MatchManager.OnScored += MatchManager_OnScored;
        MatchManager.OnCompleteResetAfterScore += MatchManager_OnCompleteResetAfterScore;
    }

	private void OnDestroy(){
		MatchManager.OnScored -= MatchManager_OnScored;
		MatchManager.OnCompleteResetAfterScore -= MatchManager_OnCompleteResetAfterScore;
	}

    private void OnTriggerEnter2D(Collider2D _collider2D)
    {
        if (_collider2D.tag == Disc.Instance.tag)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;

            SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
            Color color = spr.color;

            iTween.ValueTo(gameObject, iTween.Hash(
                "from", spr.color.a,
                "to", 0,
                "time", 1.0f,
                "onupdate",
                    (System.Action<object>)(value =>
                    {
                        color.a = (float)value;
                        spr.color = color;
                    }),
                "oncomplete",
                    (System.Action<object>)(p =>
                    {
						Destroy(gameObject);
                        Destroy(transform.parent.gameObject);
                    })
                ));
        }
    }

    #endregion

    #region Callbacks

    private void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        gameObject.transform.GetComponent<Collider2D>().enabled = false;
    }

    private void MatchManager_OnCompleteResetAfterScore(object sender, System.EventArgs e)
    {
		gameObject.transform.GetComponent<Collider2D>().enabled = true;
    }

    #endregion
}